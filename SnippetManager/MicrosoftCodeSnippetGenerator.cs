using System;

namespace SnippetManager
{
    public class MicrosoftCodeSnippetGenerator
    {
        readonly Snippet snippet;
        readonly string template;

        public MicrosoftCodeSnippetGenerator(Snippet snippet, string template)
        {
            this.snippet = snippet;
            this.template = template;
        }

        public string GetSnippetCode()
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
