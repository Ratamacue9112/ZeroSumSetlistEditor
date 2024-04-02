using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        public string Artist { get; set; }

        public StatisticsViewModel(string artist)
        {
            Artist = artist;
        }
    }
}
