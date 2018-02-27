using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class VisualStudioCodeSnippetGenerator : ISnippetGenerator
    {
        readonly IEnumerable<Snippet> snippets;

        public VisualStudioCodeSnippetGenerator(IEnumerable<Snippet> snippets)
        {
            this.snippets = snippets;
        }

        public IEnumerable<(string fileName, string code)> GetCodeSnippets(string template)
        {
            return snippets.Select(snippet => (snippet.Shortcut + ".snippet", GetSnippetCode(template, snippet)));
        }

        string GetSnippetCode(string template, Snippet snippet)
        {
            return template
                .Replace("{Title}", snippet.Shortcut)
                .Replace("{Shortcut}", snippet.Shortcut)
                .Replace("{Description}", snippet.Description)
                .Replace("{Author}", "UnKnown")
                .Replace("{Code}", string.Join(Environment.NewLine, snippet.GetSnippetCode()));
        }
    }
}