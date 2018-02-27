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
        public bool ExistSelectedMarker { get; }
        public bool ExistEndMarker { get; }

        public Snippet(string title, IEnumerable<string> codeLines)
        {
            this.title = title;
            this.codeLines = codeLines;
            Description = GetDescription();
            Shortcut = title;//Shortcut and Title are the same due to めんどくさい.
            ExistSelectedMarker = ExistMarker(Const.SelectedMarker);
            ExistEndMarker = ExistMarker(Const.EndMarker);
        }

        string GetDescription()
        {
            return codeLines
                .Select(line => line.Trim())
                .Where(trim => trim.StartsWith(Const.DescriptionTag))
                .Select(trim => trim.Remove(0, Const.DescriptionTag.Length).Trim())
                .FirstOrDefault();
        }

        bool ExistMarker(string marker)
        {
            return codeLines.Any(line => line.Contains(marker) || line.Contains(marker.ToUpper()));
        }

        public IEnumerable<string> GetSnippetCode(string selected, string end)
        {
            return codeLines
                .Where(line => !line.Trim().StartsWith(Const.DescriptionTag))
                .Where(line => !line.Trim().StartsWith(Const.SnippetTag))
                .Select(line => line.Replace(Const.SelectedMarker, selected))
                .Select(line => line.Replace(Const.SelectedMarker.ToUpper(), selected))
                .Select(line => line.Replace(Const.EndMarker, end))
                .Select(line => line.Replace(Const.EndMarker.ToUpper(), end));
        }

        enum State
        {
            Keep,
            Remove,
        }

        public bool TryGetSnippetRemovedNestedSnippet(out Snippet snippet)
        {
            if (codeLines.All(line => !line.Trim().StartsWith(Const.SnippetTag)))
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

            snippet = new Snippet(title + Const.OnlySnippetSuffix, ret);
            return true;
        }

        public bool Equals(Snippet other)
        {
            return other != null && title == other.title && codeLines.SequenceEqual(other.codeLines);
        }
    }
}