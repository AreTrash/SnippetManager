﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SnippetManager
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!TryReadAllText(Const.SettingsFileName, out var settingFileText))
            {
                Console.WriteLine($"Not Found \"{Const.SettingsFileName}\".");
                Console.WriteLine("Put it in the same directory as the application.");
                Console.ReadLine();
                return;
            }

            var settingReader = new SettingReader(settingFileText.Split('\n'));
            var snippets = GetAllSnippets(settingReader.CodeFolderPath);

            var errors = snippets.OfType<ErrorSnippet>();
            foreach (var errorSnippet in errors) Console.WriteLine(errorSnippet.FormattedErrorMessage);
            var snippetsExceptError = snippets.Except(errors);

            var vsCodeSnippetGenerator = new VisualStudioCodeSnippetGenerator(snippetsExceptError);
            WriteCodeSnippets(vsCodeSnippetGenerator, settingReader.VSCodeSnippetFolderPath);
            Console.WriteLine();

            var rsLiveTemplateGenerator = new ReSharperLiveTemplateGenerator(snippetsExceptError);
            WriteCodeSnippets(rsLiveTemplateGenerator, settingReader.RSLiveTemplateFolderPath);
            Console.WriteLine();

            Console.WriteLine("ALL Done");
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
                .Select(path => (path: path, lines: File.ReadAllLines(path)));

            var defaultSnippet = files
                .SelectMany(file =>
                {
                    return new CodeReader(file.path, file.lines)
                        .GetSnippetInfos()
                        .Select(s => (path : file.path, snippet : s));
                });

            var duplicated = defaultSnippet.GroupBy(t => t.snippet).Where(g => g.Count() >= 2).SelectMany(g => g);
            var duplicatedError = duplicated.Select(t => new ErrorSnippet(t.snippet.Title, t.path, "Duplicate shortcut name."));

            var defaultSnippetAdjustDuplicated = defaultSnippet.Select(t => t.snippet)
                .Except(duplicated.Select(t => t.snippet))
                .Concat(duplicatedError);

            var snippetRemoveNested = defaultSnippetAdjustDuplicated
                .Select(s => (canGet: s.TryGetSnippetRemovedNestedSnippet(out var snippet), snippet: snippet))
                .Where(tup => tup.canGet)
                .Select(tup => tup.snippet);

            return defaultSnippetAdjustDuplicated.Concat(snippetRemoveNested);
        }

        static void WriteCodeSnippets(ISnippetGenerator snippetGenerator, string folderPath)
        {
            var di = new DirectoryInfo(folderPath);
            Console.WriteLine($"[{snippetGenerator.GetType().Name}]");
            Console.WriteLine($"Location: {di.FullName}");
            Console.WriteLine("Create this? [Y/N]");

            if (Console.ReadLine()?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("It was canceled.");
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var (fileName, xDoc) in snippetGenerator.GetCodeSnippets())
            {
                xDoc.Save($@"{folderPath}\{fileName}");
            }

            Console.WriteLine("Create successfully.");
        }
    }
}