using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetManager
{
    interface ISnippetGenerator
    {
        IEnumerable<(string fileName, string code)> GetCodeSnippets(string template);
    }
}
