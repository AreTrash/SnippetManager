using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SnippetManager
{
    public class ReSharperLiveTemplateGenerator
    {
        readonly Snippet snippet;
        readonly string template;

        public ReSharperLiveTemplateGenerator(Snippet snippet, string template)
        {
            this.snippet = snippet;
            this.template = template;
        }

        public string GetSnippetCode()
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
    }
}
