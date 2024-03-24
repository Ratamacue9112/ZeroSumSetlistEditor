using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class SongNote
    {
        public string _note = string.Empty;
        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
                HasChanged = true;
            }
        }

        public string Role { get; set; }
        public bool HasChanged { get; set; }

        public SongNote(string role, string note)
        {
            Note = note;
            Role = role;
            HasChanged = false;
        }
    }

    public class Song : IComparable
    {
        public string Name { get; set; }
        public List<string> Notes { get; set; }
        public string Artist { get; set; }

        public Song(string name, List<string> notes, string artist)
        {
            Name = name;
            Notes = notes;
            Artist = artist;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not Song) return 1;

            return Name.CompareTo((obj as Song)!.Name);
        }
    }
}
