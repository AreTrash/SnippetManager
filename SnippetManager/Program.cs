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
            if (!TryReadAllText(Const.SettingsFileName, out var settingFileText))
            {
                Console.WriteLine($"Not Found {Const.SettingsFileName}. Make {Const.SettingsFileName} in directory same as application(.exe).");
                return;
            }

            var settingReader = new SettingReader(settingFileText.Split('\n'));
            var snippets = GetAllSnippets(settingReader.CodeFolderPath).ToArray();

            WriteCodeSnippets(
                new VisualStudioCodeSnippetGenerator(snippets),
                Const.VisualStudioCodeSnippetFileTemplateName,
                settingReader.VisualStudioCodeSnippetFolderPath,
                "VisualStudioCodeSnippet\n{0}\nCreate This?"
            );

            WriteCodeSnippets(
                new ReSharperLiveTemplateGenerator(snippets),
                Const.ReSharperLiveTemplateTemplateName,
                settingReader.ReSharperLiveTemplateFolderPath,
                "ReSharperLiveTemplate\n{0}\n Create This?"
            );

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static bool TryReadAllText(string filePath, out string allText)
        {
            if (File.Exists(filePath))
            {
                allText = File.ReadAllText(filePath);
                return true;
            }
            else
            {
                allText = null;
                return false;
            }
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

        static void WriteCodeSnippets(ISnippetGenerator snippetGenerator, string templateName, string folderPath, string message)
        {
            var di = new DirectoryInfo(folderPath);
            Console.WriteLine($"{string.Format(message, di.FullName)}");
            Console.WriteLine("[Y/N]");

            if (Console.ReadLine()?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("Creation was canceled");
                return;
            }
            if (!TryReadAllText(templateName, out var template))
            {
                Console.WriteLine($"Not Found {Const.SettingsFileName}. Make {Const.SettingsFileName} in directory same as application(.exe).");
                return;
            }

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            foreach (var (fileName, code) in snippetGenerator.GetCodeSnippets(template))
            {
                File.WriteAllText($@"{folderPath}\{fileName}", code);
            }

            Console.WriteLine("Create Successfully");
        }
    }
}