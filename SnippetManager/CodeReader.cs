using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class CodeReader
    {
        readonly string path;
        readonly IEnumerable<string> codeLines;

        public CodeReader(string path, IEnumerable<string> codeLines)
        {
            this.path = path;
            this.codeLines = codeLines;
        }

        public IEnumerable<Snippet> GetSnippetInfos()
        {
            var usedRows = new HashSet<int>();

            foreach (var (line, i) in codeLines.Select((line, i) => (line, i)))
            {
                if (!IsSnippetTag(line, out var title) || usedRows.Contains(i)) continue;
                yield return GetSnippetInfo(title, i + 1, out var endRow);
                usedRows.Add(endRow);
            }
        }

        Snippet GetSnippetInfo(string title, int startRow, out int endRow)
        {
            if (codeLines.Skip(startRow).All(line => !IsSnippetEndTag(line, title)))
            {
                endRow = -1;
                return new ErrorSnippet(title, path, "Not exist snippet end tag.");
            }

            var snippetLines = codeLines
                .Skip(startRow)
                .TakeWhile(line => !IsSnippetEndTag(line, title))
                .ToArray();

            endRow = startRow + snippetLines.Length;
            return new Snippet(title, snippetLines);
        }

        bool IsSnippetTag(string line, out string title)
        {
            var trim = line.Trim();

            if (trim.StartsWith(Const.SnippetTag))
            {
                title = trim.Remove(0, Const.SnippetTag.Length).Trim();
                return true;
            }
            else
            {
                title = null;
                return false;
            }
        }

        bool IsSnippetEndTag(string line, string title)
        {
            return IsSnippetTag(line, out var _title) && _title == title;
        }
    }
}