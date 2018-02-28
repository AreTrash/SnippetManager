using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnippetManager
{
    interface ISnippetGenerator
    {
        IEnumerable<(string fileName, XDocument xDocument)> GetCodeSnippets();
    }
}