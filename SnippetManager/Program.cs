using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnippetManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var settingReader = GetSettingReader();
            var snippets = GetAllSnippets(settingReader.CodeFolderPath).ToArray();

            Console.WriteLine($"Create VisualStudio Code Snippets in {Directory.GetCurrentDirectory()}{settingReader.VSSnippetFolderPath}");
            Console.WriteLine("[Y/N]");
            if (Console.ReadLine()?.Trim() == "Y") WriteMsCodeSnippets(snippets, settingReader.VSSnippetFolderPath);

            Console.WriteLine($"Create ReSharper Live Template in {Directory.GetCurrentDirectory()}{settingReader.VSSnippetFolderPath}");
            Console.WriteLine("[Y/N]");
            if (Console.ReadLine()?.Trim() == "Y") WriteReSharperLiveTemplates(snippets);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void SettingFileCheck(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"{fileName}が見つかりません。実行ファイルと同じディレクトリに{fileName}を作成してください");
            }
        }

        static void CreateDirectoryIfEmpty(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        static SettingReader GetSettingReader()
        {
            SettingFileCheck(Const.SettingsFileName);
            return new SettingReader(File.ReadAllLines(Const.SettingsFileName));
        }

        static IEnumerable<Snippet> GetAllSnippets(string codeFolderPath)
        {
            var files = Directory
                .GetFiles(codeFolderPath, "*.cs", SearchOption.AllDirectories)
                .Select(File.ReadAllLines)
                .ToArray();

            var defaultSnippet = files
                .SelectMany(file => new CodeReader(file).GetSnippetInfos())
                .ToArray();

            var snippetRemoveNested = defaultSnippet
                .Select(s => (canGet: s.TryGetSnippetRemovedNestedSnippet(out var snippet), snippet: snippet))
                .Where(tup => tup.canGet)
                .Select(tup => tup.snippet);

            return defaultSnippet.Concat(snippetRemoveNested);
        }

        static void WriteMsCodeSnippets(IEnumerable<Snippet> snippets, string visualStudioSnippetFolderPath)
        {
            SettingFileCheck(Const.VisualStudioCodeSnippetFileTemplateName);

            var template = File.ReadAllText(Const.VisualStudioCodeSnippetFileTemplateName);

            const string MsCodeSnippets = "MsCodeSnippets";
            CreateDirectoryIfEmpty(MsCodeSnippets);

            var willCreateVisualStudioSnippet = !string.IsNullOrWhiteSpace(visualStudioSnippetFolderPath);
            if (willCreateVisualStudioSnippet) CreateDirectoryIfEmpty(visualStudioSnippetFolderPath);

            foreach (var snippet in snippets)
            {
                var mcsg = new MicrosoftCodeSnippetGenerator(snippet, template);
                File.WriteAllText($@"{MsCodeSnippets}\{snippet.Shortcut}.snippet", mcsg.GetSnippetCode());

                if (willCreateVisualStudioSnippet)
                {
                    File.WriteAllText($@"{visualStudioSnippetFolderPath}\{snippet.Shortcut}.snippet", mcsg.GetSnippetCode());
                }
            }
        }

        static void WriteReSharperLiveTemplates(IEnumerable<Snippet> snippets)
        {
            SettingFileCheck(Const.ReSharperLiveTemplateTemplateName);

            var template = File.ReadAllText(Const.ReSharperLiveTemplateTemplateName);

            const string ReShapeLiveTemplatesFolder = "RsLiveTemplates";
            CreateDirectoryIfEmpty(ReShapeLiveTemplatesFolder);

            var sb = new StringBuilder();
            sb.Append("<wpf:ResourceDictionary xml:space=\"preserve\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:ss=\"urn:shemas-jetbrains-com:settings-storage-xaml\" xmlns:wpf=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            foreach (var snippet in snippets)
            {
                var rsltg = new ReSharperLiveTemplateGenerator(snippet, template);
                sb.Append("\n" + rsltg.GetSnippetCode());
            }
            sb.Append("</wpf:ResourceDictionary>");

            File.WriteAllText($@"{ReShapeLiveTemplatesFolder}\RsLiveTemplates.DotSettings", sb.ToString());
        }
    }
}
