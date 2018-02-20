using Xunit;

namespace SnippetManager.Test
{
    public class SnippetTest
    {
        [Fact]
        public void Description()
        {
            var exitDescSnippet = new Snippet("Hoge", new[] { "//@ Hello, World!!", "AAA", });
            Assert.Equal("Hello, World!!", exitDescSnippet.Description);

            var nonDescSnippet = new Snippet("Hoge", new[] { "AAA", });
            Assert.Null(nonDescSnippet.Description);
        }

        [Fact]
        public void GetSimpleSnippetCode()
        {
            var snippet = new Snippet("Hoge", new[] { "AAA", "BBB", "CCC" });
            Assert.Equal(new[] { "AAA", "BBB", "CCC" }, snippet.GetSnippetCode());
        }
    }
}
