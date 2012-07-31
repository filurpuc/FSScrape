using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mittan.FS;
using System.IO;

namespace FSScrapeTest
{
    [TestFixture]
    class FSParserFixture
    {
        [Test]
        public void ParseSchema_ValidSchema()
        {
            string filename = @"tmp\AllaPass1.html";
            IFSParser parser = new FSHtmlParser();
            IList<Pass> pass = parser.ParseSchema(File.ReadAllText(filename));

            //Assert.IsTrue(pass.Count > 0);
        }
    }
}
