using Xunit;

namespace SnippetManager.Test
{
    public class SnippetTest
    {
        [Fact]
        public void Description()
        {
            var existDescSnippet = new Snippet("Hoge", new[] { "//@ Hello, World!!", "AAA", });
            Assert.Equal("Hello, World!!", existDescSnippet.Description);

            var notExistDescSnippet = new Snippet("Hoge", new[] {"AAA",});
            Assert.Null(notExistDescSnippet.Description);
        }

        [Fact]
        public void Shortcut()
        {
            var existOtherSnippet = new Snippet("Hoge", new []{"//$Fuga", "AAA", "//$Fuga"});
            Assert.Equal("HogeFull", existOtherSnippet.Shortcut);

            var notExistOtherSnippet = new Snippet("Hoge", new[] {"AAA",});
            Assert.Equal("Hoge", notExistOtherSnippet.Shortcut);
        }

        [Fact]
        public void GetSimpleSnippetCode()
        {
            var snippet = new Snippet("Hoge", new[] { "AAA", "BBB", "CCC" });
            Assert.Equal(new[] { "AAA", "BBB", "CCC" }, snippet.GetSnippetCode());
        }

        [Fact]
        public void GetSnippetCodeExistDescription()
        {
            var snippet = new Snippet("Hoge", new []{"//@ Hello, World!", "AAA"});
            Assert.Equal(new[] {"AAA"}, snippet.GetSnippetCode());
        }

        [Fact]
        public void GetSnippetCodeExistNestedSnippet()
        {
            var codeLines = new[]
            {
                "AAA",
                "//$Fuga",
                "BBB",
                "//$Fuga",
                "CCC",
            };
            var snippet = new Snippet("Hoge", codeLines);
            Assert.Equal(new[] {"AAA", "BBB", "CCC"}, snippet.GetSnippetCode());
        }

        [Fact]
        public void TryGetSnippetRemovedNestedSnippet_NOTExistNested()
        {
            var codeLines = new[]
            {
                "AAA",
                "BBB",
                "CCC",
            };
            var snippet = new Snippet("Hoge", codeLines);
            Assert.False(snippet.TryGetSnippetRemovedNestedSnippet(out var _));
        }

        [Fact]
        public void TryGetSnippetRemovedNestedSnippet_ExistSimpleNested()
        {
            var codeLines = new[]
            {
                "AAA",
                "//$Fuga",
                "BBB",
                "//$Fuga",
                "CCC",
            };
            var oldSnippet = new Snippet("Hoge", codeLines);
            Assert.True(oldSnippet.TryGetSnippetRemovedNestedSnippet(out var newSnippet));
            Assert.Equal(new Snippet("Hoge", new[] {"AAA", "CCC"}), newSnippet);
        }

        [Fact]
        public void TryGetSnippetRemovedNestedSnippet_ExistComplexNested()
        {
            var codeLines = new[]
            {
                "XXX",
                "//$Fuga",
                "YYY",
                "//$Piyo",
                "ZZZ",
                "//$Piyo",
                "//$Fuga",
                "AAA",
                "//$NonEnd",
                "PPP",
            };
            var oldSnippet = new Snippet("Hoge", codeLines);
            Assert.True(oldSnippet.TryGetSnippetRemovedNestedSnippet(out var newSnippet));
            Assert.Equal(new Snippet("Hoge", new[] { "XXX", "AAA" }), newSnippet);
        }
    }
}
