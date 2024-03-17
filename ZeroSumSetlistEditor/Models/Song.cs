using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class Song
    {
        public string Name { get; set; }
        public Dictionary<string, string> Notes { get; set; }

        public Song(string name, Dictionary<string, string> notes)
        {
            Name = name;
            Notes = notes;
        }
    }
}
