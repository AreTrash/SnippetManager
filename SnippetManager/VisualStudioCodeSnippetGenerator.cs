using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SnippetManager
{
    public class VisualStudioCodeSnippetGenerator : ISnippetGenerator
    {
        static XDocument XDocMain => new XDocument(new XDeclaration("1.0", "utf-8", null),
            new XElement("CodeSnippets",
                new XElement("CodeSnippet", new XAttribute("Format", "1.0.0"),
                    new XElement("Header",
                        new XElement("Title"),
                        new XElement("Shortcut"),
                        new XElement("Description"),
                        new XElement("Author"),
                        new XElement("SnippetTypes",
                            new XElement("SnippetType", "Expansion")
                        )
                    ),
                    new XElement("Snippet",
                        new XElement("Declarations"),
                        new XElement("Code", new XAttribute("Language", "csharp"))
                    )
                )
            )
        );

        static XContainer Literal => new XElement("Literal",
            new XElement("ID"),
            new XElement("Default")
        );

        readonly IEnumerable<Snippet> snippets;

        public VisualStudioCodeSnippetGenerator(IEnumerable<Snippet> snippets)
        {
            this.snippets = snippets;
        }

        public IEnumerable<(string fileName, XDocument xDocument)> GetCodeSnippets()
        {
            return snippets.Select(snippet =>
            {
                var fileName = snippet.Shortcut + ".snippet";
                var xDocument = new XDocumentGenerator(snippet).GetSnippetXDocument(XDocMain);
                return (fileName, xDocument);
            });
        }

        class XDocumentGenerator
        {
            readonly Snippet snippet;

            public XDocumentGenerator(Snippet snippet)
            {
                this.snippet = snippet;
            }

            public XDocument GetSnippetXDocument(XDocument xDoc)
            {
                AddContainers(xDoc);
                DeleteEmptyElement(xDoc);
                AddXmlns(xDoc);
                return xDoc;
            }

            void AddContainers(XDocument xDoc)
            {
                xDoc.Descendants("Title").Single().Add(snippet.Shortcut);
                xDoc.Descendants("Shortcut").Single().Add(snippet.Shortcut);
                xDoc.Descendants("Description").Single().Add(snippet.Description);

                var code = string.Join(Environment.NewLine, snippet.GetSnippetCode("$selected$", "$end$"));
                xDoc.Descendants("Code").Single().Add(new XCData(code));

                if (snippet.ExistSelectedMarker)
                {
                    xDoc.Descendants("SnippetTypes").Single().Add(new XElement("SnippetType", "SurroundsWith"));
                }
            }

            void DeleteEmptyElement(XContainer container)
            {
                //XXX If don't ToArray(), iterator will be broken.
                foreach (var child in container.Elements().ToArray()) DeleteEmptyElement(child);
                if (container is XElement element && element.IsEmpty) container.Remove();
            }

            void AddXmlns(XContainer container)
            {
                XNamespace xmlns = "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet";
                foreach (var node in container.Descendants()) node.Name = xmlns + node.Name.LocalName;
            }
        }
    }
}