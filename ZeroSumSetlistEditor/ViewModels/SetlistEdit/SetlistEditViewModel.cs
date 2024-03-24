using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;
using ZeroSumSetlistEditor.Views;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSong
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string DisplayColor { get; set; }

        public SetlistSong(string name, int number, string displayColor)
        {
            Name = name;
            Number = number;
            DisplayColor = displayColor;
        }
    }

    public class SetlistEditViewModel : ViewModelBase
    {
        public ObservableCollection<SetlistSong> Songs { get; set; }

        public Setlist Setlist { get; set; }
        public string Artist { get; set; }

        public Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?> ShowDialog { get; }

        private MainWindowViewModel mainWindowVm;

        public SetlistEditViewModel(Setlist setlist, List<string> songs, MainWindowViewModel mainWindowVm)
        {
            var list = new ObservableCollection<SetlistSong>();
            for (int i = 0; i < songs.Count; i++)
            {
                list.Add(new SetlistSong(songs[i], i + 1, GetDisplayColor(i)));
            }
            Songs = list;
            Setlist = setlist;
            Artist = setlist.Artist;
            ShowDialog = new Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?>();
            this.mainWindowVm = mainWindowVm;
        }

        public void RemoveSong(SetlistSong song)
        {
            Songs.Remove(song);
        } 

        public string GetDisplayColor(int number)
        {
            return number % 2 == 0 ? "#6E6E6E" : "#5E5E5E";
        }

        public async void OpenAddSongDialog()
        {
            var window = new SetlistAddSongWindowViewModel(Artist, mainWindowVm.fileReading.GetSongs(Artist));
            var result = await ShowDialog.Handle(window);
        }

        public void Save()
        {
            mainWindowVm.fileReading.SaveSetlist(Setlist, Songs.ToList());
            mainWindowVm.OpenSetlistSelect(Artist);
        }

        public async void Cancel()
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Do you want to leave without saving? Changes will be lost.", ButtonEnum.YesNo);

            var result = await box.ShowAsync();
            if (result.ToString() == "Yes")
            {
                mainWindowVm.OpenSetlistSelect(Artist);
            }
        }
    }
}
