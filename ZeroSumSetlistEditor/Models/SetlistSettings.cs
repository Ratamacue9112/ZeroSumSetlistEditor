using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class SetlistSettings
    {
        public bool Initialized = false;
        
        public bool _hasChanged { get; set; }
        public bool HasChanged
        {
            get
            {
                return _hasChanged;
            }
            set
            {
                if (Initialized)
                {
                    _hasChanged = value;
                }
                else
                {
                    _hasChanged = false;
                }
            }
        }

        public Color _backgroundColor { get; set; }
        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                HasChanged = true;
                _backgroundColor = value;
            }
        }

        public Color _songColor { get; set; }
        public Color SongColor
        {
            get
            {
                return _songColor;
            }
            set
            {
                HasChanged = true;
                _songColor = value;
            }
        }

        public Color _noteColor { get; set; }
        public Color NoteColor
        {
            get
            {
                return _noteColor;
            }
            set
            {
                HasChanged = true;
                _noteColor = value;
            }
        }

        public Color _intermissionColor { get; set; }
        public Color IntermissionColor
        {
            get
            {
                return _intermissionColor;
            }
            set
            {
                HasChanged = true;
                _intermissionColor = value;
            }
        }

        public Color _encoreColor { get; set; }
        public Color EncoreColor
        {
            get
            {
                return _encoreColor;
            }
            set
            {
                HasChanged = true;
                _encoreColor = value;
            }
        }

        public string _fontFamily { get; set; } = "";
        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                HasChanged = true;
                _fontFamily = value;
            }
        }

        public int _headerSize { get; set; }
        public int HeaderSize
        {
            get
            {
                return _headerSize;
            }
            set
            {
                HasChanged = true;
                _headerSize = value;
            }
        }

        public int _songSize { get; set; }
        public int SongSize
        {
            get
            {
                return _songSize;
            }
            set
            {
                HasChanged = true;
                _songSize = value;
            }
        }

        public int _noteSize { get; set; }
        public int NoteSize
        {
            get
            {
                return _noteSize;
            }
            set
            {
                HasChanged = true;
                _noteSize = value;
            }
        }

        public int _intermissionSize { get; set; }
        public int IntermissionSize
        {
            get
            {
                return _intermissionSize;
            }
            set
            {
                HasChanged = true;
                _intermissionSize = value;
            }
        }

        public int _encoreSize { get; set; }
        public int EncoreSize
        {
            get
            {
                return _encoreSize;
            }
            set
            {
                HasChanged = true;
                _encoreSize = value;
            }
        }

        public bool _showVenue { get; set; }
        public bool ShowVenue
        {
            get
            {
                return _showVenue;
            }
            set
            {
                HasChanged = true;
                _showVenue = value;
            }
        }

        public bool _showDate { get; set; }
        public bool ShowDate
        {
            get
            {
                return _showDate;
            }
            set
            {
                HasChanged = true;
                _showDate = value;
            }
        }

        public bool _showArtist { get; set; }
        public bool ShowArtist
        {
            get
            {
                return _showArtist;
            }
            set
            {
                HasChanged = true;
                _showArtist = value;
            }
        }

        public void ResetToDefaults()
        {
            Initialized = false;
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
            Initialized = true;
        }
    }
}
