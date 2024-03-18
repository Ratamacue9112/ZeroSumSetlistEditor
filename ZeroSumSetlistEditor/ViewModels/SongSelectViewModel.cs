using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SongSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public ObservableCollection<Song> Songs { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public Interaction<CreateWindowViewModel, SongSelectViewModel?> ShowDialog { get; }

        public SongSelectViewModel(string artist, List<Song> songs, List<string> roles)
        {
            Artist = artist;
            Songs = new ObservableCollection<Song>(songs);
            Roles = new ObservableCollection<string>(roles);
            ShowDialog = new Interaction<CreateWindowViewModel, SongSelectViewModel?>();
        }

        public async void OpenCreateSongDialog(string song)
        {
            var window = new CreateWindowViewModel(Artist, song == "" ? CreateWindowMode.CreateSong : CreateWindowMode.EditSong, song, Roles.Count);
            var result = await ShowDialog.Handle(window);
        }
    }
}
