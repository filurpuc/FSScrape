using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mittan.FS
{
    public interface IFSParser
    {
        Pass ParsePass(string html);
        IList<Pass> ParseSchema(string html);
    }
}
