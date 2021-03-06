﻿using System.Collections.Generic;
using System.Linq;

namespace SnippetManager
{
    public class SettingReader
    {
        readonly IEnumerable<string> settings;

        public string CodeFolderPath { get; }
        public string VSCodeSnippetFolderPath { get; }
        public string RSLiveTemplateFolderPath { get; }

        public SettingReader(IEnumerable<string> settings)
        {
            this.settings = settings;
            CodeFolderPath = ReadStringSetting(Const.CodeFolderPathSettingName);
            VSCodeSnippetFolderPath = ReadStringSetting(Const.VisualStudioCodeSnippetFolderPathSettingName);
            RSLiveTemplateFolderPath = ReadStringSetting(Const.ReSharperLiveTemplateFolderPathSettingName);
        }

        string ReadStringSetting(string name)
        {
            var formattedSettings = settings
                .Where(line => line.Contains('='))
                .Select(line => line.Split('='))
                .Select(sp => (name: sp[0].Trim(), body: sp[1].Trim().Trim('"')));

            return formattedSettings
                .Where(fs => fs.name == name)
                .Select(fs => fs.body)
                .FirstOrDefault();
        }
    }
}