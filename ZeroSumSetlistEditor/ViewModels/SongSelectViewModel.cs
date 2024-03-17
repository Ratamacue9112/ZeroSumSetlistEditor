using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SongSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }

        public SongSelectViewModel(string artist)
        {
            Artist = artist;
        }
    }
}
