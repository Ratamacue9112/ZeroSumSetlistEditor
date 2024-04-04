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

        private MainWindowViewModel mainWindowVm;

        public StatisticsViewModel(string artist, MainWindowViewModel mainWindowVm)
        {
            Artist = artist;
            this.mainWindowVm = mainWindowVm;
        }

        public void RescanStatistics()
        {
            mainWindowVm.fileReading.RescanStatistics(Artist);
        }
    }
}
