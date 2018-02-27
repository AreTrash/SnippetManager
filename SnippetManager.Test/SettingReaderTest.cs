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
                "NotExistEqualLine",
                "CodeFolderPath=\"hogehoge\"",
                "VSSnippetFolderPath = fugafuga",
                "RSLiveTemplateFolderPath=\" piyopiyo \""
            };

            var sr = new SettingReader(settings);
            Assert.Equal("hogehoge", sr.CodeFolderPath);
            Assert.Equal("fugafuga", sr.VSCodeSnippetFolderPath);
            Assert.Equal(" piyopiyo ", sr.RSLiveTemplateFolderPath);
        }

        [Fact]
        public void SettingIfEmpty()
        {
            var sr = new SettingReader(new string[0]);
            Assert.Null(sr.CodeFolderPath);
        }
    }
}
