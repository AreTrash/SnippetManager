using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SnippetManager
{
    public class ReSharperLiveTemplateGenerator : ISnippetGenerator
    {
        static readonly XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        static readonly XNamespace s = "clr-namespace:System;assembly=mscorlib";
        static readonly XNamespace ss = "urn:shemas-jetbrains-com:settings-storage-xaml";
        static readonly XNamespace wpf = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        static XDocument XDocMain => new XDocument(
            new XElement(wpf + "ResourceDictionary",
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XAttribute(XNamespace.Xmlns + "x", x),
                new XAttribute(XNamespace.Xmlns + "s", s),
                new XAttribute(XNamespace.Xmlns + "ss", ss),
                new XAttribute(XNamespace.Xmlns + "wpf", wpf)
            )
        );

        readonly IEnumerable<Snippet> snippets;

        public ReSharperLiveTemplateGenerator(IEnumerable<Snippet> snippets)
        {
            this.snippets = snippets;
        }

        public IEnumerable<(string fileName, XDocument xDocument)> GetCodeSnippets()
        {
            var xDoc = XDocMain;

            var children = snippets.SelectMany(snippet => new XElementGenerator(snippet).GetXElements());
        
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once CoVariantArrayConversion
            xDoc.Element(wpf + "ResourceDictionary").Add(children);

            yield return ("RsLiveTemplates.DotSettings", xDoc);
        }

        class XElementGenerator
        {
            readonly Snippet snippet;

            string Hash1 { get; }
            string Hash2 { get; }
            string TypeText { get; }

            public XElementGenerator(Snippet snippet)
            {
                this.snippet = snippet;
                (Hash1, Hash2) = GetHash();
                TypeText = GetTypeText();
            }

            (string, string) GetHash()
            {
                var md5 = new MD5CryptoServiceProvider();
                var hash1Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(snippet.Shortcut));
                var hash1 = new string(hash1Bytes.SelectMany(b => b.ToString("x2")).ToArray()).ToUpper();
                var hash2Bytes = md5.ComputeHash(hash1Bytes);
                var hash2 = new string(hash2Bytes.SelectMany(b => b.ToString("x2")).ToArray()).ToUpper();
                return (hash1, hash2);
            }

            string GetTypeText()
            {
                switch (snippet.IndentCount)
                {
                    case 0: case 1: return "InCSharpTypeAndNamespace";
                    case 2: return "InCSharpTypeMember";
                    default: return "InCSharpExpression";
                }
            }

            public IEnumerable<XElement> GetXElements()
            {
                yield return GetElem(GetKey("@KeyIndexDefined"), true);
                yield return GetElem(GetKey("Shortcut/@EntryValue"), snippet.Shortcut);
                yield return GetElem(GetKey("Description/@EntryValue"), snippet.Description);
                yield return GetElem(GetKey("Text/@EntryValue"), string.Join("\n", snippet.GetSnippetCode("$SELECTION$", "$END$")));
                yield return GetElem(GetKey("IsBlessed/@EntryValue"), true);
                yield return GetElem(GetKey("Reformat/@EntryValue"), true);
                yield return GetElem(GetKey("ShortenQualifiedReferences/@EntryValue"), true);
                yield return GetElem(GetKey("Applicability/=Live/@EntryIndexedValue"), true);
                yield return GetElem(GetKey("Applicability/=Surround/@EntryIndexedValue"), snippet.ExistSelectedMarker);
                yield return GetElem(GetKey("Scope", "@KeyIndexDefined"), true);
                yield return GetElem(GetKey("Scope", "Type/@EntryValue"), TypeText);
                yield return GetElem(GetKey("Scope", "CustomProperties/=minimumLanguageVersion/@EntryIndexedValue"), "2.0");

                foreach (var (param, i) in snippet.Parameters.Select((param, i) => (param, i)))
                {
                    yield return GetElem(GetKey($"Field/={param}/@KeyIndexDefined"), true);
                    yield return GetElem(GetKey($"Field/={param}/Order/@EntryValue"), i);
                }
            }

            string GetKey(string key1)
            {
                return $"/Default/PatternsAndTemplates/LiveTemplates/Template/={Hash1}/{key1}";
            }

            string GetKey(string key1, string key2)
            {
                return $"{GetKey(key1)}/={Hash2}/{key2}";
            }

            XElement GetElemCore(string tag, string key, object value)
            {
                return new XElement(s + tag, new XAttribute(x + "Key", key), value);
            }

            XElement GetElem(string key, bool boolean)
            {
                return GetElemCore("Boolean", key, boolean.ToString());
            }

            XElement GetElem(string key, int number)
            {
                return GetElemCore("Int64", key, number);
            }

            XElement GetElem(string key, string text)
            {
                return GetElemCore("String", key, text);
            }
        }
    }
}