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
        public DocumentGeneration documentGeneration;

        private ViewModelBase content;

        public MainWindowViewModel()
        {
            fileReading = new FileReading();
            documentGeneration = new DocumentGeneration();

            content = ArtistSelect = new ArtistSelectViewModel(fileReading.GetArtists());
        }

        public void OpenModeSelect(string artist)
        {
            Content = new ModeSelectViewModel(artist);
        }

        public void OpenArtistSelect()
        {
            Content = ArtistSelect = new ArtistSelectViewModel(fileReading.GetArtists());
            RegisterShowCreateArtistDialog.Invoke();
        }

        public void OpenSongSelect(string artist)
        {
            Content = SongSelect = new SongSelectViewModel(artist, fileReading.GetSongs(artist), fileReading.GetRoles(artist), this);
            RegisterShowCreateSongDialog.Invoke();
        }

        public void OpenRoleEdit(string artist)
        {
            Content = RoleEdit = new RoleEditViewModel(artist, fileReading.GetRoles(artist), this);
            RegisterShowCreateRoleDialog.Invoke();
        }

        public void OpenNoteEdit(Song song)
        {
            Content = new NoteEditViewModel(song, SongSelect.Roles.ToList(), this);
        }

        public void OpenSetlistSelect(string artist)
        {
            Content = SetlistSelect = new SetlistSelectViewModel(artist, fileReading.GetSetlists(artist), this);
            RegisterShowCreateSetlistDialog.Invoke();
        }

        public void OpenSetlistSettings(string artist)
        {
            Content = new SetlistSettingsViewModel(artist, fileReading.GetSetlistSettings(), this);
        }

        public void OpenSetlistEdit(Setlist setlist)
        {
            Content = SetlistEdit = new SetlistEditViewModel(setlist, fileReading.GetSetlistSongs(setlist), this);
            RegisterShowSetlistAddSongDialog.Invoke();
        }

        public void OpenSetlistDocumentGenerate(Setlist setlist)
        {
            Content = new SetlistDocumentGenerateViewModel(setlist, fileReading.GetRoles(setlist.Artist), fileReading.GetSetlistSongsFullDetail(setlist), this);
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public Action RegisterShowCreateArtistDialog { get; set; }
        public Action RegisterShowCreateSongDialog { get; set; }
        public Action RegisterShowCreateRoleDialog { get; set; }
        public Action RegisterShowCreateSetlistDialog { get; set; }
        public Action RegisterShowSetlistAddSongDialog { get; set; }

        public ArtistSelectViewModel ArtistSelect { get; set; }
        public SongSelectViewModel SongSelect { get; set; }
        public RoleEditViewModel RoleEdit { get; set; }
        public SetlistSelectViewModel SetlistSelect { get; set; }
        public SetlistEditViewModel SetlistEdit { get; set; }
    }
}
