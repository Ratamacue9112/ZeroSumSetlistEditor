using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class ModeSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }

        public ModeSelectViewModel(string artist)
        {
            Artist = artist;
        }
    }
}
