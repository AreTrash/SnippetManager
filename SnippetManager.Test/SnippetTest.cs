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
    }
}
