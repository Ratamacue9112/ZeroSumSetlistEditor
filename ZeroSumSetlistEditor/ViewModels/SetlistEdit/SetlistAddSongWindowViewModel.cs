using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistAddSongWindowViewModel : ViewModelBase
    {
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

        public ObservableCollection<Song> Songs { get; set; }
        public ObservableCollection<Song> FilteredSongs { get; set; }
        public string SelectedSong { get; set; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public SetlistAddSongWindowViewModel(string artist, List<Song> songs)
        {
            Songs = new ObservableCollection<Song>(songs);
            FilteredSongs = new ObservableCollection<Song>(songs);
            SelectedSong = "";
        }

        public void FilterSongs()
        {
            FilteredSongs.Clear();
            foreach (Song s in Songs)
            {
                if (s.Name.StartsWith(SearchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    FilteredSongs.Add(s);
                }
            }
        }

        public void Add(string song)
        {
            SelectedSong = song;
            CloseDialog?.Invoke();
        }
    }
}
