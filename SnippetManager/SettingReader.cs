using System.Collections.Generic;

namespace SnippetManager
{
    public class SettingReader
    {
        public string CodeFolderPath => ReadStringSetting(nameof(CodeFolderPath));
        public string VisualStudioSnippetFolderPath => ReadStringSetting(nameof(VisualStudioSnippetFolderPath));
        public string ReShaperSettingPath => ReadStringSetting(nameof(ReShaperSettingPath));
        public bool UpdateVisualStudioSnippet => ReadBoolSetting(nameof(UpdateVisualStudioSnippet));
        public bool UpdateReShaperSetting => ReadBoolSetting(nameof(UpdateReShaperSetting));

        readonly IReadOnlyCollection<string> settings;

        public SettingReader(string[] settings)
        {
            this.settings = settings;
        }

        string ReadStringSetting(string name)
        {
            foreach (var fs in GetFormattedSettings())
            {
                if (fs.name == name) return fs.body;
            }
            return null;
        }

        bool ReadBoolSetting(string name)
        {
            foreach (var fs in GetFormattedSettings())
            {
                if (fs.name == name) return bool.Parse(fs.body);
            }
            return false;
        }

        IEnumerable<(string name, string body)> GetFormattedSettings()
        {
            foreach (var setting in settings)
            {
                var sp = setting.Split('=');
                yield return (sp[0].Trim(' '), sp[1].Trim(' ').Trim('"'));
            }
        }
    }
}
