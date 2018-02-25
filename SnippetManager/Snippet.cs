using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class Snippet : IEquatable<Snippet>
    {
        readonly string title;
        readonly IReadOnlyCollection<string> codeLines;

        public string Description { get; }

        public Snippet(string title, IReadOnlyCollection<string> codeLines)
        {
            this.title = title;
            this.codeLines = codeLines;
            Description = GetDescription();
        }

        string GetDescription()
        {
            foreach (var line in codeLines)
            {
                var trim = line.Trim(' ', '\t');
                if (trim.StartsWith(Const.DescriptionTag))
                {
                    return trim.Remove(0, Const.DescriptionTag.Length).Trim(' ');
                }
            }
            return null;
        }

        public IEnumerable<string> GetSnippetCode()
        {
            return codeLines;
        }

        public bool Equals(Snippet other)
        {
            return other != null && title == other.title && codeLines.SequenceEqual(other.codeLines);
        }
    }
}
