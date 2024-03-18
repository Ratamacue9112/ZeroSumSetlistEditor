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
        public List<string> Notes { get; set; }

        public Song(string name, List<string> notes)
        {
            Name = name;
            Notes = notes;
        }
    }
}
