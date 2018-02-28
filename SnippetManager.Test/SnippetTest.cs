using Xunit;

namespace SnippetManager.Test
{
    public class SnippetTest
    {
        [Fact]
        public void Description()
        {
            var existDescriptionSnippet = new Snippet("Hoge", new[] { "//@ Hello, World!!", "AAA", });
            Assert.Equal("Hello, World!!", existDescriptionSnippet.Description);

            var notExistDescriptionSnippet = new Snippet("Hoge", new[] {"AAA",});
            Assert.Null(notExistDescriptionSnippet.Description);
        }

        [Fact]
        public void Shortcut()
        {
            var existNestedSnippet = new Snippet("Hoge", new []{"//$Fuga", "AAA", "//$Fuga"});
            Assert.Equal("Hoge", existNestedSnippet.Shortcut);

            existNestedSnippet.TryGetSnippetRemovedNestedSnippet(out var removeNestedSnippet);
            Assert.Equal("HogeOnly", removeNestedSnippet.Shortcut);

            var notExistOtherSnippet = new Snippet("Hoge", new[] {"AAA",});
            Assert.Equal("Hoge", notExistOtherSnippet.Shortcut);
        }

        [Fact]
        public void ExistMarker()
        {
            Assert.True(new Snippet("Hoge", new []{"/*$selected$*/"}).ExistSelectedMarker);
            Assert.True(new Snippet("Hoge", new []{"/*$END$*/"}).ExistEndMarker);
            Assert.True(new Snippet("Hoge", new[] { "ooo/*$SELECTED$*/ooo" }).ExistSelectedMarker);
            Assert.False(new Snippet("Hoge", new[] { "/*$selected$*/" }).ExistEndMarker);
        }

        [Fact]
        public void Parameter()
        {
            Assert.Equal(new [] {"xxx"}, new Snippet("Hoge", new []{"__xxx__"}).Parameters);
        }

        [Fact]
        public void GetSimpleSnippetCode()
        {
            var snippet = new Snippet("Hoge", new[] { "AAA", "BBB", "CCC" });
            Assert.Equal(new[] { "AAA", "BBB", "CCC" }, snippet.GetSnippetCode(null, null));
        }

        [Fact]
        public void GetSnippetCodeExistDescription()
        {
            var snippet = new Snippet("Hoge", new []{"//@ Hello, World!", "AAA"});
            Assert.Equal(new[] {"AAA"}, snippet.GetSnippetCode(null, null));
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
            Assert.Equal(new[] {"AAA", "BBB", "CCC"}, snippet.GetSnippetCode(null, null));
        }

        [Fact]
        public void GetSnippetCodeExistSelectedAndEndMarker()
        {
            var codeLines = new[]
            {
                "/*$selected$*/",
                "/*$SELECTED$*/",
                "  /*$end$*/  ",
                "oo/*$END$*/oo",
                "/*$SeLeCtEd$*/",
                "/* $END$ */",
                "OOO",
            };
            var snippet = new Snippet("Hoge", codeLines);

            var expected = new[]
            {
                "$S$",
                "$S$",
                "  $E$  ",
                "oo$E$oo",
                "/*$SeLeCtEd$*/",
                "/* $END$ */",
                "OOO",
            };
            Assert.Equal(expected, snippet.GetSnippetCode("$S$", "$E$"));
        }

        [Fact]
        public void GetSnippetCodeExistParameter()
        {
            Assert.Equal(new[] {"$apple$"}, new Snippet("Hoge", new[] {"__apple__"}).GetSnippetCode(null, null));
            Assert.Equal(new[] {"_apple_"}, new Snippet("Hoge", new[] {"_apple_"}).GetSnippetCode(null, null));
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
            Assert.Equal(new Snippet("HogeOnly", new[] {"AAA", "CCC"}), newSnippet);
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
            Assert.Equal(new Snippet("HogeOnly", new[] { "XXX", "AAA" }), newSnippet);
        }
    }
}
