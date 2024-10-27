using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.ViewModels;

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
        public string ShortName { get; set; }
        public List<string> Notes { get; set; }
        public string Artist { get; set; }

        public Song(string name, string shortName, List<string> notes, string artist)
        {
            Name = name;
            ShortName = shortName;
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

    public partial class SetlistSong : ObservableObject
    {
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private string _shortName = "";
        [ObservableProperty]
        private string _shortNameDisplay = "";
        [ObservableProperty]
        private int _number = 0;
        [ObservableProperty]
        private string _numberText = "";
        [ObservableProperty]
        private string _displayColor = "";
        [ObservableProperty]
        private SetlistItemType _type = SetlistItemType.Song;
        [ObservableProperty]
        private string _oneOffNote = "";
        [ObservableProperty]
        private string _oneOffNoteDisplay = "";

        public SetlistSong(string name, string shortName, int number, string displayColor, SetlistItemType type, string oneOffNote)
        {
            Name = name;
            ShortName = shortName;
            ShortNameDisplay = shortName != name ? "[" + shortName + "]" : "";
            Number = number;
            if (type == SetlistItemType.Song) NumberText = number.ToString() + ". ";
            else NumberText = string.Empty;
            DisplayColor = displayColor;
            Type = type;
            OneOffNote = oneOffNote;
            if (OneOffNote != "")
            {
                OneOffNoteDisplay = "(" + OneOffNote + ")";
            }
        }
    }
}
