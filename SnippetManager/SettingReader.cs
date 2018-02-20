using System.Collections.Generic;

namespace SnippetManager
{
    public class SettingReader
    {
        public string CodeFolderPath { get; }
        public string VisualStudioSnippetFolderPath { get; }

        readonly IReadOnlyCollection<string> settings;

        public SettingReader(string[] settings)
        {
            this.settings = settings;
            CodeFolderPath = ReadStringSetting(nameof(CodeFolderPath));
            VisualStudioSnippetFolderPath = ReadStringSetting(nameof(VisualStudioSnippetFolderPath));
        }

        string ReadStringSetting(string name)
        {
            foreach (var fs in GetFormattedSettings())
            {
                if (fs.name == name) return fs.body;
            }
            return null;
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
