using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public ObservableCollection<Song> FilteredSongs { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                FilterSongs();
            }
        }

        public Interaction<CreateWindowViewModel, SongSelectViewModel?> ShowDialog { get; }

        private MainWindowViewModel mainWindowVm;

        public SongSelectViewModel(string artist, List<Song> songs, List<string> roles, MainWindowViewModel mainWindowVm)
        {
            Artist = artist;
            Songs = new ObservableCollection<Song>(songs);
            FilteredSongs = new ObservableCollection<Song>(songs);
            Roles = new ObservableCollection<string>(roles);
            ShowDialog = new Interaction<CreateWindowViewModel, SongSelectViewModel?>();
            this.mainWindowVm = mainWindowVm;
        }

        public async void OpenCreateSongDialog(string song)
        {
            var window = new CreateWindowViewModel(Artist, song == "" ? CreateWindowMode.CreateSong : CreateWindowMode.EditSong, song, Roles.Count, mainWindowVm);
            var result = await ShowDialog.Handle(window);
        }

        public async void RemoveSong(string song)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Delete Song",
                ContentMessage = "Really delete song \"" + song + "\"? Notes will be deleted and cannot be recovered.",
                ButtonDefinitions = new List<ButtonDefinition>() {
                    new ButtonDefinition { Name = "Yes" },
                    new ButtonDefinition { Name = "No" }
                }
            });

            var result = await box.ShowAsync();
            if (result == "Yes")
            {
                foreach (Song i in Songs)
                {
                    if (i.Name == song)
                    {
                        Songs.Remove(i);
                        break;
                    }
                }
                Songs.Sort();

                mainWindowVm.fileReading.RemoveSong(song, Artist);
            }
            FilterSongs();
        }
    
        public void FilterSongs()
        {
            FilteredSongs.Clear();
            foreach (Song s in Songs)
            {
                if (s.Name.ToLower().StartsWith(SearchText.ToLower()))
                {
                    FilteredSongs.Add(s);
                }
            }
        }
    }
}
