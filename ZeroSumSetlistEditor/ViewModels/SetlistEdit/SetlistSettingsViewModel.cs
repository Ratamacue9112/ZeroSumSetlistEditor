using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSettings
    {
        public Color BackgroundColor { get; set; }
        public Color SongColor { get; set; }
        public Color NoteColor { get; set; }
        public Color IntermissionColor { get; set; }
        public Color EncoreColor { get; set; }
        public string FontFamily { get; set; } = "";
        public int HeaderSize { get; set; }
        public int SongSize { get; set; }
        public int NoteSize { get; set; }
        public int IntermissionSize { get; set; }
        public int EncoreSize { get; set; }
        public bool ShowVenue { get; set; }
        public bool ShowDate { get; set; }
        public bool ShowArtist { get; set; }

        public void ResetToDefaults()
        {
            BackgroundColor = Colors.White;
            SongColor = Colors.Black;
            NoteColor = Colors.Black;
            IntermissionColor = Colors.Black;
            EncoreColor = Colors.Black;
            FontFamily = "";
            HeaderSize = 20;
            SongSize = 16;
            NoteSize = 16;
            IntermissionSize = 16;
            EncoreSize = 16;
            ShowVenue = true;
            ShowDate = true;
            ShowArtist = false;
        }
    }

    public class SetlistSettingsViewModel : ViewModelBase
    {
        public string Artist { get; set; }

        public SetlistSettings Settings { get; set; }

        public SetlistSettingsViewModel(string artist)
        {
            Artist = artist;

            Settings = new SetlistSettings();
            Settings.ResetToDefaults();
        }
    }
}
