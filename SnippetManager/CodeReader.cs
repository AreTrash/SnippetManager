using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    /// <summary>
    /// ソースコードを読み取りスニペットを取得する
    /// </summary>
    public class CodeReader
    {
        readonly IEnumerable<string> codeLines;

        public CodeReader(IEnumerable<string> codeLines)
        {
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
            var snippetLines = codeLines
                .Skip(startRow)
                .TakeWhile(line => !IsSnippetTag(line, out var _title) || _title != title)
                .ToArray();

            endRow = startRow + snippetLines.Length;
            return new Snippet(title, snippetLines);
        }

        bool IsSnippetTag(string line, out string title)
        {
            var trim = line.Trim();
            title = trim.Remove(0, Const.SnippetTag.Length).Trim();
            return trim.StartsWith(Const.SnippetTag);
        }
    }
}

