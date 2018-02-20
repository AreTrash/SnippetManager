using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SnippetManager
{
    public class Snippet : IEquatable<Snippet>
    {
        public string Title { get; }
        public string Description { get; }

        const string DiscriptionTag = "//@";
        readonly IReadOnlyCollection<string> codeLines;

        public Snippet(string title, IReadOnlyCollection<string> codeLines)
        {
            Title = title;
            this.codeLines = codeLines;
            Description = GetDescription();
        }

        string GetDescription()
        {
            foreach (var line in codeLines)
            {
                var trim = line.Trim(' ', '\t');
                if (trim.StartsWith(DiscriptionTag))
                {
                    return trim.Remove(0, DiscriptionTag.Length).Trim(' ');
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
            return other != null && Title == other.Title && codeLines.SequenceEqual(other.codeLines);
        }
    }
}
