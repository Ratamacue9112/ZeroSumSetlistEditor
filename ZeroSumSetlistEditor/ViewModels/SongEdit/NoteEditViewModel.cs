using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
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
        public ObservableCollection<SongNote> Notes { get; set; }
        public string SongName { get; set; }
        public string ShortSongName { get; set; }
        public string Artist { get; set; }

        private MainWindowViewModel mainWindowVm { get; }

        public NoteEditViewModel(Song song, List<string> roles, MainWindowViewModel mainWindowVm) 
        {
            Notes = new ObservableCollection<SongNote>();
            for (int i = 0; i < roles.Count; i++)
            {
                Notes.Add(new SongNote(roles[i], song.Notes[i]));
            }

            SongName = song.Name;

            if (song.ShortName == string.Empty)
            {
                ShortSongName = "";
            }
            else
            {
                ShortSongName = "(" + song.ShortName + ")";
            }

            Artist = song.Artist;
            this.mainWindowVm = mainWindowVm;
        }

        public void SaveNotes()
        {
            foreach (SongNote note in Notes)
            {
                if (note.Note.Contains(','))
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Can't use commas in notes. Either remove them or use a substitute like semicolons.", ButtonEnum.Ok);

                    var result = box.ShowAsync();
                    return;
                }
            }
            mainWindowVm.fileReading.SaveNotes(Artist, SongName, Notes.ToList());
            mainWindowVm.OpenSongSelect(Artist);
        }

        public async void CancelChanges()
        {
            bool hasChanged = false;
            foreach (var note in Notes)
            {
                if (note.HasChanged)
                {
                    hasChanged = true;
                    break;
                }
            }
            if (hasChanged)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Do you want to leave without saving? Changes will be lost.", ButtonEnum.YesNo);
                var result = await box.ShowAsync();
                if (result.ToString() == "No") return;
            }

            mainWindowVm.OpenSongSelect(Artist);
        }
    }
}
