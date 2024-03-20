using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class Setlist : IComparable
    {
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string DateText { get; set; }

        public Setlist(string venue, DateTime date)
        {
            Venue = venue;
            Date = date;
            DateText = date.ToString("dddd, dd{0} MMMM yyyy");
            DateText = string.Format(DateText, Date.Day.GetOrdinalSuffix());
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not Setlist) return 1;

            return Date.CompareTo(((Setlist)obj).Date) * -1;
        }
    }
}
