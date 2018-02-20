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
                "VisualStudioSnippetFolderPath=fugafuga",
                "  ReShaperSettingPath  =  \" piyo piyo \"  ",
                "UpdateVisualStudioSnippet=true",
                "UpdateReShaperSetting=\"false\""
            };

            var sr = new SettingReader(settings);
            Assert.Equal("hogehoge", sr.CodeFolderPath);
            Assert.Equal("fugafuga", sr.VisualStudioSnippetFolderPath);
            Assert.Equal(" piyo piyo ", sr.ReShaperSettingPath);
            Assert.True(sr.UpdateVisualStudioSnippet);
            Assert.False(sr.UpdateReShaperSetting);
        }

        [Fact]
        public void SettingIfEmpty()
        {
            var sr = new SettingReader(new string[0]);
            Assert.Null(sr.CodeFolderPath);
            Assert.False(sr.UpdateVisualStudioSnippet);
        }
    }
}
