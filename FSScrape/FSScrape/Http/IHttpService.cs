using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mittan.FS.Http
{
    public interface IHttpService
    {
        string Get(string url, string content);
        string Post(string url, string content);
    }
}
