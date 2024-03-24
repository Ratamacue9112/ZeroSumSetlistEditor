using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public FileReading fileReading;

        private ViewModelBase content;

        public MainWindowViewModel()
        {
            fileReading = new FileReading();

            content = ArtistSelect = new ArtistSelectViewModel(fileReading.GetArtists());
        }

        public void OpenModeSelect(string artist)
        {
            Content = new ModeSelectViewModel(artist);
        }

        public void OpenArtistSelect()
        {
            Content = new ArtistSelectViewModel(fileReading.GetArtists());
        }

        public void OpenSongSelect(string artist)
        {
            Content = SongSelect = new SongSelectViewModel(artist, fileReading.GetSongs(artist), fileReading.GetRoles(artist), this);
            ShowCreateSongDialog.Invoke();
        }

        public void OpenRoleEdit(string artist)
        {
            Content = RoleEdit = new RoleEditViewModel(artist, fileReading.GetRoles(artist), this);
            ShowCreateRoleDialog.Invoke();
        }

        public void OpenNoteEdit(Song song)
        {
            Content = new NoteEditViewModel(song, SongSelect.Roles.ToList(), this);
        }

        public void OpenSetlistSelect(string artist)
        {
            Content = SetlistSelect = new SetlistSelectViewModel(artist, fileReading.GetSetlists(artist), this);
            ShowCreateSetlistDialog.Invoke();
        }

        public void OpenSetlistEdit(Setlist setlist)
        {
            Content = SetlistEdit = new SetlistEditViewModel(setlist, fileReading.GetSetlistSongs(setlist), this);
            ShowSetlistAddSongDialog.Invoke();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public Action ShowCreateSongDialog { get; set; }
        public Action ShowCreateRoleDialog { get; set; }
        public Action ShowCreateSetlistDialog { get; set; }
        public Action ShowSetlistAddSongDialog { get; set; }

        public ArtistSelectViewModel ArtistSelect { get; }
        public SongSelectViewModel SongSelect { get; set; }
        public RoleEditViewModel RoleEdit { get; set; }
        public SetlistSelectViewModel SetlistSelect { get; set; }
        public SetlistEditViewModel SetlistEdit { get; set; }
    }
}
