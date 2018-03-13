using System.Linq;
using Xunit;

namespace SnippetManager.Test
{
    public class CodeReaderTest
    {
        [Fact]
        public void GetSimpleSnippetInfo()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Hoge",
                "YYY",
                "//$Hoge",
                "ZZZ",
            };

            var exp = new[] {new Snippet("Hoge", new[] {"YYY"})};
            var act = new CodeReader("path", codeLines).GetSnippetInfos();
            Assert.Equal(exp, act);
        }

        [Fact]
        public void GetMultipleSnippetInfo()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Hoge",
                "YYY",
                "//$Hoge",
                "ZZZ",
                "//$Fuga",
                "AAA",
                "BBB",
                "CCC",
                "//$Fuga",
                "//$Piyo",
                "//$Piyo",
            };

            var exp = new[]
            {
                new Snippet("Hoge", new[] {"YYY"}),
                new Snippet("Fuga", new[] {"AAA", "BBB", "CCC"}),
                new Snippet("Piyo", new string[0]),
            };
            var act = new CodeReader("path", codeLines).GetSnippetInfos();
            Assert.Equal(exp, act);
        }

        [Fact]
        public void GetNonEndSnippetInfo()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Hoge",
                "YYY",
                "ZZZ",
                "//$Piyo",
                "AAA",
            };

            var exp = new[]
            {
                new ErrorSnippet("Hoge", "path", "Not exist snippet end tag."),
                new ErrorSnippet("Piyo", "path", "Not exist snippet end tag."),
            };
            var act = new CodeReader("path", codeLines).GetSnippetInfos();
            Assert.Equal(exp, act);
        }

        [Fact]
        public void GetNestedSnippetInfo()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Hoge",
                "YYY",
                "//$Fuga",
                "OOO",
                "//$Fuga",
                "//$Hoge",
                "ZZZ",
            };

            var exp = new[]
            {
                new Snippet("Hoge", new[] {"YYY", "//$Fuga", "OOO", "//$Fuga"}), 
                new Snippet("Fuga", new []{"OOO"}), 
            };
            var act = new CodeReader("path", codeLines).GetSnippetInfos();
            Assert.Equal(exp, act);
        }

        [Fact]
        public void GetDuplicatedSnippetInfo()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Hoge",
                "YYY",
                "//$Hoge",
                "OOO",
                "//$Hoge",
                "ZZZ",
                "//$Hoge",
            };

            var exp = new[]
            {
                new Snippet("Hoge", new[] {"YYY"}),
                new Snippet("Hoge", new[] {"ZZZ"}),
            };
            var act = new CodeReader("path", codeLines).GetSnippetInfos();
            Assert.Equal(exp, act);
        }
    }
}