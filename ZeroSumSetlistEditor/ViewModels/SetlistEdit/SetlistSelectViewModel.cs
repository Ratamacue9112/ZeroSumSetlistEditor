using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public ObservableCollection<Setlist> Setlists { get; set; }

        public SetlistSelectViewModel(string artist, List<Setlist> setlists)
        {
            Artist = artist;
            Setlists = new ObservableCollection<Setlist>(setlists);
        }
    }
}
