using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class SetlistChange
    {
    }

    public class SetlistAddDeleteChange : SetlistChange
    {
        public string SongName { get; set; }
        public bool Deleted { get; set; }

        public SetlistAddDeleteChange(string name, bool deleted)
        {
            SongName = name;
            Deleted = deleted;
        }
    }

    public enum SetlistMainPosition
    {
        ShowOpener,
        MainSetCloser,
        ShowCloser
    }

    public class SetlistMainPositionChange : SetlistChange
    {
        public string OldSongName { get; set; }
        public string NewSongName { get; set; }
        public SetlistMainPosition Position { get; set; }

        public SetlistMainPositionChange(string oldSongName, string newSongName, SetlistMainPosition position)
        {
            OldSongName = oldSongName;
            NewSongName = newSongName;
            Position = position;
        }
    }

    public class SetlistEmptyStateChange : SetlistChange
    {
        public bool Empty { get; set; }

        public SetlistEmptyStateChange(bool empty)
        {
            Empty = empty;
        }
    }
}
