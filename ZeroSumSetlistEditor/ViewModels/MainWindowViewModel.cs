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
        private FileReading fileReading;

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
            Content = SongSelect = new SongSelectViewModel(artist, fileReading.GetSongs(artist), fileReading.GetRoles(artist));
            ShowCreateSongDialog.Invoke();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public Action ShowCreateSongDialog { get; set; }

        public ArtistSelectViewModel ArtistSelect { get; }
        
        public SongSelectViewModel SongSelect { get; set; }
    }
}
