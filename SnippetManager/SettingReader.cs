using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class SettingReader
    {
        readonly IReadOnlyCollection<string> settings;

        public string CodeFolderPath { get; }
        public string VisualStudioSnippetFolderPath { get; }

        public SettingReader(IReadOnlyCollection<string> settings)
        {
            this.settings = settings;
            CodeFolderPath = ReadStringSetting(nameof(CodeFolderPath));
            VisualStudioSnippetFolderPath = ReadStringSetting(nameof(VisualStudioSnippetFolderPath));
        }

        string ReadStringSetting(string name)
        {
            var formattedSettings = settings
                .Where(line => line.Contains('='))
                .Select(line => line.Split('='))
                .Select(sp => (name: sp[0].Trim(' '), body: sp[1].Trim(' ').Trim('"')));

            return formattedSettings
                .Where(fs => fs.name == name)
                .Select(fs => fs.body)
                .FirstOrDefault();
        }
    }
}
