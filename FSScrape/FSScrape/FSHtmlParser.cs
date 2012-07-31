using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Mittan.FS
{
    public class FSHtmlParser : IFSParser
    {
        // Matchar ett datum på formatet "2012-01-07 kl 10:30 - 11:25".
        private const string FSDatumRegExp = @"^([\d]{4}-[\d]{2}-[\d]{2})[^\d]+([\d]{2}:[\d]{2})[^\d]+([\d]{2}:[\d]{2}).*";

        private const string FSDatumRegExp = @"^(.+\s+)([\d]{4}-[\d]{2}-[\d]{2})";

        // Matchar antal bokningar och antal bokningsbara platser ("25 / 25").
        private const string FSBokningarRegExp = @"^([\d]+)[^\d]+([\d]+)";

        public Pass ParsePass(string html)
        {
            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(html);

            HtmlNode td = doc.DocumentNode.SelectSingleNode("//td[@id='content']");
            HtmlNode[] h1s = td.SelectNodes(".//h1").ToArray();

            Pass pass = new Pass();

            pass.Passtyp = h1s[0].InnerText.Trim();

            // 2012-01-07 kl 10:30 - 11:25
            Trace.WriteLine("Datum/tid: " + h1s[1].InnerText.Trim());
            Tuple<DateTime, DateTime> datum = ParseDatumOchTid(h1s[1].InnerText.Trim());

            pass.Starttid = datum.Item1;
            pass.Sluttid = datum.Item2;

            HtmlNode[] spans = td.SelectNodes(".//span").ToArray();

            
            Trace.WriteLine("Bokningar: " + spans[1].InnerText.Trim());
            Tuple<int, int> bokningar = ParseBokningar(spans[1].InnerText.Trim());

            pass.Bokningar = bokningar.Item1;
            pass.MaxBokningar = bokningar.Item2;
            pass.Plats = spans[2].InnerText.Trim();
            pass.Anlänt = int.Parse(spans[3].InnerText.Trim());
            pass.Sal = spans[4].InnerText.Trim();
            pass.MaxAntal = int.Parse(spans[5].InnerText.Trim());
            pass.Ledare = spans[6].InnerText.Trim();
            return pass;
        }

        /// <summary>
        /// Skapar start- och slutdatum från ett F&S-datum på formatet "2012-01-07 kl 10:30 - 11:25"
        /// </summary>
        /// <param name="date">Datumsträng</param>
        /// <returns>En tuple med start- och slutdatum</returns>
        private Tuple<DateTime, DateTime> ParseDatumOchTid(string fsdatum)
        {
            Regex exp = new Regex(FSDatumRegExp, RegexOptions.IgnoreCase);
            MatchCollection MatchList = exp.Matches(fsdatum);
            Match firstMatch = MatchList[0];
            string datum = firstMatch.Groups[1].ToString();
            DateTime startdatum = DateTime.Parse(datum + " " + firstMatch.Groups[2]);
            DateTime slutdatum = DateTime.Parse(datum + " " + firstMatch.Groups[3]);

            return Tuple.Create<DateTime, DateTime>(startdatum, slutdatum);
        }

        /// <summary>
        /// Må 2012-04-09
        /// </summary>
        /// <param name="fsdatum"></param>
        /// <returns></returns>
        private Tuple<DateTime, DateTime> ParseDatum(string fsdatum)
        {
            Regex exp = new Regex(FSDatumRegExp, RegexOptions.IgnoreCase);
            MatchCollection MatchList = exp.Matches(fsdatum);
            Match firstMatch = MatchList[0];
            string datum = firstMatch.Groups[1].ToString();
            DateTime startdatum = DateTime.Parse(datum + " " + firstMatch.Groups[2]);
            DateTime slutdatum = DateTime.Parse(datum + " " + firstMatch.Groups[3]);

            return Tuple.Create<DateTime, DateTime>(startdatum, slutdatum);
        }

        
        /// <summary>
        /// Extraherar antal bokningar och antal bokningsbara platser från FS-formatet.
        /// </summary>
        /// <param name="fsbokningar"></param>
        /// <returns>En tuple med antal bokningar och antal bokningsbara platser</returns>
        private Tuple<int, int> ParseBokningar(string fsbokningar)
        {
            Regex exp = new Regex(FSBokningarRegExp, RegexOptions.IgnoreCase);
            MatchCollection MatchList = exp.Matches(fsbokningar);
            Match firstMatch = MatchList[0];
            int antalBokningar = int.Parse(firstMatch.Groups[1].ToString());
            int antalBokningsbara = int.Parse(firstMatch.Groups[2].ToString());

            return Tuple.Create<int, int>(antalBokningar, antalBokningsbara);
        }

        public IList<Pass> ParseSchema(string html)
        {
            IList<Pass> list = new List<Pass>();
            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(html);

            HtmlNodeCollection resultatlista = doc.DocumentNode.SelectNodes("//div[@id='res_list_area']/div");

            foreach (HtmlNode node in resultatlista)
            {
                // "div_tid_0"
                Trace.WriteLine(node.Attributes.AttributesWithName("id").First().Value);

                // On 2012-04-11
                Trace.WriteLine(node.SelectSingleNode("./table/tr/td").InnerText.Trim());

                HtmlNodeCollection passRader = node.SelectNodes("./table[@class='resultlist']/tbody/tr");
               
                Trace.WriteLine(passRader.Count);

                foreach (HtmlNode n in passRader)
                {
                    Trace.WriteLine(n.SelectSingleNode("td[@class='sort_col1']").InnerText.Trim());
                    Trace.WriteLine(n.SelectSingleNode("td[@class='sort_col5']").InnerText.Trim());
                }
            }

            return list;
        }
    }
}
