using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mittan.FS
{
    public class Pass
    {
        public string Passid { get; set; }
        public string Passtyp { get; set; }
        public DateTime Starttid { get; set; }
        public DateTime Sluttid { get; set; }
        public int Bokningar { get; set; }
        public int MaxBokningar { get; set; }
        public string Plats { get; set; }
        public int Anlänt { get; set; }
        public int MaxAntal { get; set; }
        public string Ledare { get; set; }
        public string Sal { get; set; }

        public override string ToString()
        {
            return new StringBuilder().Append("Plats: ").Append(Plats).Append("\nSal: ").Append(Sal).Append("\nPassid: ").Append(Passid).Append("\nTidpunkt: ").Append(Starttid).Append(" - ").Append(Sluttid).Append("\nPasstyp: ").Append(Passtyp).Append("\nLedare: ").Append(Ledare).Append("\nBokningar: ").Append(Bokningar).Append("/").Append(MaxBokningar).ToString();
        }
    }
}
