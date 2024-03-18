using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class NoteEditViewModel : ViewModelBase
    {
        public class SongNote
        {
            public string Role { get; set; }
            public string Note { get; set; }

            public SongNote(string role, string note) 
            {
                Role = role;
                Note = note;
            }
        }

        public ObservableCollection<SongNote> Notes { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }

        public NoteEditViewModel(Song song, List<string> roles) 
        {
            Notes = new ObservableCollection<SongNote>();
            for (int i = 0; i < roles.Count; i++)
            {
                Notes.Add(new SongNote(roles[i], song.Notes[i]));
            }
            SongName = song.Name;
            Artist = song.Artist;
        }
    }
}
