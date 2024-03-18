using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class Setlist
    {
        public string Venue { get; set; }
        public string Date { get; set; }

        public Setlist(string venue, string date)
        {
            Venue = venue;
            Date = date;
        }
    }
}
