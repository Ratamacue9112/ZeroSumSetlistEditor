using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSong
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string DisplayColor { get; set; }

        public SetlistSong(string name, int number, string displayColor)
        {
            Name = name;
            Number = number;
            DisplayColor = displayColor;
        }
    }

    public class SetlistEditViewModel : ViewModelBase
    {
        public ObservableCollection<SetlistSong> Songs { get; set; }

        public Setlist Setlist { get; set; }
        public string Artist { get; set; }

        public SetlistEditViewModel(Setlist setlist, List<string> songs)
        {
            var list = new ObservableCollection<SetlistSong>();
            for (int i = 0; i < songs.Count; i++)
            {
                list.Add(new SetlistSong(songs[i], i + 1, i % 2 == 0 ? "#7A7A7A" : "#5E5E5E"));
            }
            Songs = list;
            Setlist = setlist;
            Artist = setlist.Artist;
        }
    }
}
