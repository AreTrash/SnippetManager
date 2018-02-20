using Xunit;

namespace SnippetManager.Test
{
    public class SettingReaderTest
    {
        [Fact]
        public void SettingIfExist()
        {
            var settings = new[]
            {
                "CodeFolderPath=\"hogehoge\"",
                "VisualStudioSnippetFolderPath = fugafuga",
                "UpdateVisualStudioSnippet=true",
            };

            var sr = new SettingReader(settings);
            Assert.Equal("hogehoge", sr.CodeFolderPath);
            Assert.Equal("fugafuga", sr.VisualStudioSnippetFolderPath);
        }

        [Fact]
        public void SettingIfEmpty()
        {
            var sr = new SettingReader(new string[0]);
            Assert.Null(sr.CodeFolderPath);
        }
    }
}
