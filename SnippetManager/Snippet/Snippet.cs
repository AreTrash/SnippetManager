using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SnippetManager
{
    public class Snippet : IEquatable<Snippet>
    {
        readonly string title;
        readonly IEnumerable<string> codeLines;

        public string Title => title;

        public string Description { get; }
        public string Shortcut { get; }
        public bool ExistSelectedMarker { get; }
        public bool ExistEndMarker { get; }
        public IEnumerable<string> Parameters { get; }
        public int IndentCount { get; }

        public Snippet(string title, IEnumerable<string> codeLines)
        {
            this.title = title;
            this.codeLines = codeLines;

            Description = GetDescription();
            Shortcut = title;//Shortcut and Title are the same due to めんどくさい.
            ExistSelectedMarker = ExistMarker(Const.SelectedMarker);
            ExistEndMarker = ExistMarker(Const.EndMarker);
            Parameters = GetParameters();
            IndentCount = GetIndentCount();
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

        IEnumerable<string> GetParameters()
        {
            return codeLines
                .SelectMany(line => Regex.Matches(line, "__(.+?)__").Cast<Match>())
                .Select(match => match.Value.Trim('_'))
                .Distinct();
        }

        int GetIndentCount()
        {
            var firstLine = codeLines.FirstOrDefault();
            if (firstLine == null) return 0;
            return Math.Max(firstLine.TakeWhile(c => c == ' ').Count() / 4, firstLine.TakeWhile(c => c == '\t').Count());
        }

        public IEnumerable<string> GetSnippetCode(string selected, string end)
        {
            return codeLines
                .Where(line => !line.Trim().StartsWith(Const.DescriptionTag))
                .Where(line => !line.Trim().StartsWith(Const.SnippetTag))
                .Select(line => line.Replace(Const.SelectedMarker, selected))
                .Select(line => line.Replace(Const.SelectedMarker.ToUpper(), selected))
                .Select(line => line.Replace(Const.EndMarker, end))
                .Select(line => line.Replace(Const.EndMarker.ToUpper(), end))
                .Select(line => line.Replace("____", ""))
                .Select(line => Regex.Replace(line, "__(.+?)__", match => $"${match.Value.Trim('_')}$"));
        }

        enum State
        {
            Keep,
            Remove,
        }

        //なんかスマートな方法考えたい
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

    public class ErrorSnippet : Snippet, IEquatable<ErrorSnippet>
    {
        readonly string errorMessage;
        readonly string path;

        public string FormattedErrorMessage => GetFormattedErrorMessage();

        public ErrorSnippet(string title, string path, string errorMessage) : base(title, new []{""})
        {
            this.errorMessage = errorMessage;
            this.path = path;
        }

        string GetFormattedErrorMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Error: {errorMessage}");
            sb.AppendLine($"@Tag: {Title}");
            sb.AppendLine($"@Path: {path}");
            return sb.ToString();
        }

        public bool Equals(ErrorSnippet other)
        {
            return Title == other?.Title && errorMessage == other?.errorMessage && path == other?.path;
        }
    }
}