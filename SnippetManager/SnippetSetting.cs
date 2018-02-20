using System.Collections.Generic;

namespace SnippetManager
{
    class SnippetSetting
    {
        public static readonly IReadOnlyCollection<string> snippetPaths = new[]
        {
            "",
            "",
        };

        public static readonly bool WillWriteVisualStudioSnippet = false;
        public static readonly string VisualStudioSnippetFolderPath = "";
        public static readonly string ReshaperSettingFilePath = "";
    }
}
