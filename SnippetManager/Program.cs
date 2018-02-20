using System;
using System.IO;

namespace SnippetManager
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("Settings.txt"))
            {                                                                                                          
                throw new Exception("'Settings.txt' ファイルが見つかりません。実行ファイルと同じディレクトリにSettings.txtファイルを作成してください");
            }
        }
    }
}
