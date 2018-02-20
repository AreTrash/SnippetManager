using System;
using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class Snippet : IEquatable<Snippet>
    {
        public string Title { get; }
        public IReadOnlyCollection<string> CodeLines { get; }

        public Snippet(string title, IReadOnlyCollection<string> codeLines)
        {
            Title = title;
            CodeLines = codeLines;
        }

        public bool Equals(Snippet other)
        {
            return other != null && Title == other.Title && CodeLines.SequenceEqual(other.CodeLines);
        }
    }

    public class CodeReader
    {
        const string TagPoint = "//$";

        readonly IReadOnlyCollection<string> codeLines;

        public CodeReader(string[] codeLines)
        {
            this.codeLines = codeLines;
        }

        public IEnumerable<Snippet> GetSnippetInfos()
        {
            var usedRows = new HashSet<int>();

            foreach (var (line, i) in codeLines.Select((line, i) => (line, i)))
            {
                if (IsSnippetTag(line, out var title) && !usedRows.Contains(i))
                {
                    yield return GetSnippetInfo(title, i + 1, out var endRow);
                    usedRows.Add(endRow);
                }
            }
        }

        Snippet GetSnippetInfo(string title, int startRow, out int endRow)
        {
            var snippetLines = new List<string>();

            foreach (var (line, i) in codeLines.Skip(startRow).Select((line, i) => (line, i)))
            {
                if (IsSnippetTag(line, out var _title) && _title == title)
                {
                    endRow = i + startRow;
                    return new Snippet(title, snippetLines);
                }

                snippetLines.Add(line);
            }

            endRow = codeLines.Count;
            return new Snippet(title, snippetLines);
        }

        bool IsSnippetTag(string line, out string title)
        {
            var trim = line.Trim(' ', '\t');

            if (trim.StartsWith(TagPoint))
            {
                title = trim.Remove(0, TagPoint.Length).Trim(' ');
                return true;
            }

            title = null;
            return false;
        }
    }
}

