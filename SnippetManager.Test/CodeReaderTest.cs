using System;
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

            var infos = new CodeReader(codeLines).GetSnippetInfos().Single();
            Assert.Equal("Hoge", infos.Title);
            Assert.Equal(new[] {"YYY"}, infos.CodeLines);
        }

        [Fact]
        public void GetMultipleSnippetInfos()
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

            var infos = new CodeReader(codeLines).GetSnippetInfos().ToArray();
            Assert.Equal(new[]{"Hoge", "Fuga", "Piyo"}, infos.Select(sn => sn.Title));

            var exp = new[]
            {
                new[] {"YYY"},
                new[] {"AAA", "BBB", "CCC"},
                new string[0],
            };
            Assert.Equal(exp, infos.Select(sn => sn.CodeLines));
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
            };

            var infos = new CodeReader(codeLines).GetSnippetInfos().Single();
            Assert.Equal(new [] {"YYY", "ZZZ"}, infos.CodeLines);
        }
    }
}