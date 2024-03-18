using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
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
