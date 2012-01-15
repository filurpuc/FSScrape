using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mittan.FS
{
    interface IFSParser
    {
        Pass ParsePass(string html);
        List<Pass> ParseSchema(string html);
    }
}
