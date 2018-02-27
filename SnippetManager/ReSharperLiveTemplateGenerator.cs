using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SnippetManager
{
    /*public class ReSharperLiveTemplateGenerator : ISnippetGenerator
    {
        readonly IEnumerable<Snippet> snippets;

        public ReSharperLiveTemplateGenerator(IEnumerable<Snippet> snippets)
        {
            this.snippets = snippets;
        }

        public IEnumerable<(string fileName, string code)> GetCodeSnippets(string template)
        {
            var sb = new StringBuilder();
            sb.Append("<wpf:ResourceDictionary xml:space=\"preserve\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:ss=\"urn:shemas-jetbrains-com:settings-storage-xaml\" xmlns:wpf=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            foreach (var snippet in snippets) sb.Append("\n" + GetSnippetCode(template, snippet));
            sb.Append("</wpf:ResourceDictionary>");
            yield return ("RsLiveTemplates.DotSettings", sb.ToString());
        }

        string GetSnippetCode(string template, Snippet snippet)
        {
            var code = snippet.GetSnippetCode()
                .Select(line => line.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"))
                .Aggregate((source, line) => source + "\n" + line);

            var md5 = new MD5CryptoServiceProvider();
            var hash1Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(snippet.Shortcut));
            var hash1 = new string(hash1Bytes.SelectMany(b => b.ToString("x2")).ToArray()).ToUpper();
            var hash2Bytes = md5.ComputeHash(hash1Bytes);
            var hash2 = new string(hash2Bytes.SelectMany(b => b.ToString("x2")).ToArray()).ToUpper();

            return template
                .Replace("{Shortcut}", snippet.Shortcut)
                .Replace("{Description}", snippet.Description)
                .Replace("{Code}", code)
                .Replace("{Hash1}", hash1)
                .Replace("{Hash2}", hash2);
        }
    }*/
}