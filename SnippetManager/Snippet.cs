using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class Snippet : IEquatable<Snippet>
    {
        readonly string title;
        readonly IEnumerable<string> codeLines;

        public string Description { get; }
        public string Shortcut { get; }

        public Snippet(string title, IEnumerable<string> codeLines)
        {
            this.title = title;
            this.codeLines = codeLines;
            Description = GetDescription();
            Shortcut = GetShortcut();
        }

        string GetDescription()
        {
            return codeLines
                .Select(line => line.Trim())
                .Where(trim => trim.StartsWith(Const.DescriptionTag))
                .Select(trim => trim.Remove(0, Const.DescriptionTag.Length).Trim())
                .FirstOrDefault();
        }

        string GetShortcut()
        {
            return title + (IsIncludeOtherSnippet() ? Const.FullSnippetSuffix : "");
        }

        bool IsIncludeOtherSnippet()
        {
            return codeLines.Any(line => line.Trim().StartsWith(Const.SnippetTag));
        }

        public IEnumerable<string> GetSnippetCode()
        {
            return codeLines
                .Where(line => !line.Trim().StartsWith(Const.DescriptionTag))
                .Where(line => !line.Trim().StartsWith(Const.SnippetTag));
        }

        enum State
        {
            Keep,
            Remove,
        }

        //XXX なんか良くならないかなあ…
        public bool TryGetSnippetRemovedNestedSnippet(out Snippet snippet)
        {
            if (!IsIncludeOtherSnippet())
            {
                snippet = null;
                return false;
            }

            var ret = new List<string>();
            var state = State.Keep;
            var search = "";

            foreach (var line in codeLines)
            {
                if (state == State.Keep)
                {
                    if (line.Trim().StartsWith(Const.SnippetTag))
                    {
                        search = line.Trim();
                        state = State.Remove;
                    }
                    else
                    {
                        ret.Add(line);
                    }
                }
                else if (state == State.Remove)
                {
                    if (line.Trim() == search)
                    {
                        state = State.Keep;
                    }
                }
            }

            snippet = new Snippet(title, ret);
            return true;
        }

        public bool Equals(Snippet other)
        {
            return other != null && title == other.title && codeLines.SequenceEqual(other.codeLines);
        }
    }
}
