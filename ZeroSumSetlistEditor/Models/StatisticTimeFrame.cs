using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class StatisticTimeFrame
    {
        public string TimeFrame { get; set; }
        public Dictionary<string, int> PlayCounts { get; set; }
        public Dictionary<string, int> ShowOpeners { get; set; }
        public Dictionary<string, int> MainSetClosers { get; set; }
        public Dictionary<string, int> ShowClosers { get; set; }

        public StatisticTimeFrame() 
        {
            TimeFrame = "";
            PlayCounts = new Dictionary<string, int>();
            ShowOpeners = new Dictionary<string, int>();
            MainSetClosers = new Dictionary<string, int>();
            ShowClosers = new Dictionary<string, int>();
        }
    }
}
