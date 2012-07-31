using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using System.Net;

namespace Mittan.FS
{
    class FSScrape
    {
        private const string Passdetaljer = @"http://schema.sthlm.friskissvettis.se/index.php?id={0}&location={1}&func=rd";

        private const string SökSpinningpass = @"http://schema.sthlm.friskissvettis.se/index.php?func=fres&search=T&fromDate={0}&thruDate={1}&fromTime=07%3A00&thruTime=23%3A00&objectClasses%5BSP%5D=X&objects%5BSP_INT%5D=X&objects%5BSP_INT75%5D=X&objects%5BSP_INTVALL%5D=X&objects%5BSP_L%C5NG%5D=X&objects%5BSP_MEDEL%5D=X&objects%5BSP_PULS%5D=X&objects%5BSP_PUINTVA%5D=X&btn_submit=x#";

        private const string AllaPass = @"http://schema.sthlm.friskissvettis.se/index.php?func=la";

        private IFSParser parser;

        public void Scrape(string filename)
        {
            HtmlDocument doc = Load(filename);

            //Links(doc);
            Content(doc);
        }

        private void Content(HtmlDocument doc)
        {
            HtmlNode td = doc.DocumentNode.SelectSingleNode("//td[@id='content']");
            HtmlNode[] h1s = td.SelectNodes(".//h1").ToArray();

            Trace.WriteLine("Passtyp: " + h1s[0].InnerText.Trim());
            Trace.WriteLine("Datum/tid: " + h1s[1].InnerText.Trim());

            HtmlNode[] spans = td.SelectNodes(".//span").ToArray();

            Trace.WriteLine("Bokningar: " + spans[1].InnerText.Trim());
            Trace.WriteLine("Plats: " + spans[2].InnerText.Trim());
            Trace.WriteLine("Anlänt: " + spans[3].InnerText.Trim());
            Trace.WriteLine("Sal: " + spans[4].InnerText.Trim());
            Trace.WriteLine("Max antal: " + spans[5].InnerText.Trim());
            Trace.WriteLine("Ledare: " + spans[6].InnerText.Trim());
        }

        private void Links(HtmlDocument doc)
        {
            List<string> hrefTags = new List<string>();

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                hrefTags.Add(att.Value);
            }

            foreach (string tag in hrefTags)
            {
                Trace.WriteLine(tag);
            }
        }

        private HtmlDocument Load(string filename)
        {
            TextReader reader = File.OpenText(filename);
            HtmlDocument doc = new HtmlDocument();

            doc.Load(reader);
            reader.Close();
            return doc;
        }

        private IList<Pass> GetAllaPass()
        {
            string filename = @"tmp\Spinningpass.html";
            IFSParser parser = new FSHtmlParser();
            IList<Pass> pass = new List<Pass>();
            WebRequest request = WebRequest.Create(AllaPass);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.Default);
            string html = sr.ReadToEnd();

            stream.Close();
            return parser.ParseSchema(html);
        }

        private Pass GetPass(int passid, string lokal)
        {
            WebRequest request = WebRequest.Create(string.Format(Passdetaljer, passid, lokal));
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.Default);
            string html = sr.ReadToEnd();

            stream.Close();
            return parser.ParsePass(html);
        }


        private void GetPassTest(int id, string lokal)
        {
            string filename = @"tmp\Spinningpass.html";

            parser = new FSHtmlParser();

            Pass pass = parser.ParsePass(File.ReadAllText(filename));

            Trace.WriteLine("Pass: " + pass);

            //fs.Scrape(filename);

            Trace.WriteLine(Passdetaljer);

            for (int i = 0; i < 100; i++)
            {
                pass = GetPass(id, lokal);
                Trace.WriteLine("i=" + i + "\t" + DateTime.Now);
                if (pass.Bokningar < pass.MaxBokningar)
                {
                    Ledigtsignal(20);
                    return;
                }

                System.Threading.Thread.Sleep(3000);
            }
        }

        private void Ledigtsignal(int max)
        {
            for (int i = 0; i < max; i++)
            {
                Console.Beep();
                Trace.WriteLine("******************************************************************");
            }
        }

        static void Main(string[] args)
        {
            Trace.WriteLine("Hej");

            FSScrape fs = new FSScrape();

            //fs.GetPassTest(8301689, "LI");
            fs.GetAllaPass();

            Trace.WriteLine("Done!");
        }
    }
}
