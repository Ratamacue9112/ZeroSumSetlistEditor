using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SongSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public List<Song> Songs { get; set; }

        public SongSelectViewModel(string artist, List<Song> songs)
        {
            Artist = artist;
            Songs = songs;

            foreach(Song s in Songs)
            {
                Console.WriteLine(s.Name + ":");
                foreach (KeyValuePair<string, string> note in s.Notes)
                {
                    Console.WriteLine(note.Key + " - " + note.Value);
                }
            }
        }
    }
}
