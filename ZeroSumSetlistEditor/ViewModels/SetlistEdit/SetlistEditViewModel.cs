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
using System.Numerics;

namespace ZeroSumSetlistEditor.ViewModels
{
    public enum SetlistItemType
    {
        Song,
        Intermission,
        Encore
    }

    public class SetlistSong
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string NumberText { get; set; }
        public string DisplayColor { get; set; }
        public SetlistItemType Type { get; set; }

        public SetlistSong(string name, int number, string displayColor, SetlistItemType type)
        {
            Name = name;
            Number = number;
            if (type == SetlistItemType.Song) NumberText = number.ToString() + ". ";
            else NumberText = string.Empty; 
            DisplayColor = displayColor;
            Type = type;
        }
    }

    public class SetlistEditViewModel : ViewModelBase
    {
        public ObservableCollection<SetlistSong> Songs { get; set; }
        public int SongCount { get; set; }
        public int EncoreCount { get; set; }

        public Setlist Setlist { get; set; }
        public string Artist { get; set; }

        public bool HasChanged { get; set; }

        public Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?> ShowDialog { get; }

        private MainWindowViewModel mainWindowVm;

        private const string breakColor = "#0D777C";

        public SetlistEditViewModel(Setlist setlist, List<string> songs, MainWindowViewModel mainWindowVm)
        {
            SongCount = 0;
            EncoreCount = 0;
            var list = new ObservableCollection<SetlistSong>();
            for (int i = 0; i < songs.Count; i++)
            {
                if (songs[i].StartsWith("--") && songs[i].EndsWith("--"))
                {
                    var name = songs[i].Replace("--", "");
                    SetlistSong song = new SetlistSong(name, -1, breakColor, name == "ENCORE" ? SetlistItemType.Encore : SetlistItemType.Intermission);
                    if (song.Type == SetlistItemType.Encore)
                    {
                        EncoreCount++;
                        if (EncoreCount > 1)
                        {
                            song.Name = EncoreCount + EncoreCount.GetOrdinalSuffix() + " ENCORE";
                        }
                    }
                    list.Add(song);
                }
                else
                {
                    SongCount++;
                    list.Add(new SetlistSong(songs[i], SongCount, GetDisplayColor(SongCount), SetlistItemType.Song));
                }
            }

            Songs = list;
            Setlist = setlist;
            Artist = setlist.Artist;
            HasChanged = false;
            ShowDialog = new Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?>();
            this.mainWindowVm = mainWindowVm;
        }

        public void AdjustSongs()
        {
            HasChanged = true;
            SongCount = 0;
            EncoreCount = 0;
            for(int i = 0; i < Songs.Count; i++)
            {
                if (Songs[i].Type == SetlistItemType.Song)
                {
                    SongCount++;
                    Songs[i].Number = SongCount;
                    Songs[i].NumberText = Songs[i].Number.ToString() + ". ";
                    Songs[i].DisplayColor = GetDisplayColor(SongCount);
                }
                else 
                {
                    if (Songs[i].Type == SetlistItemType.Encore)
                    {
                        EncoreCount++;
                        if (EncoreCount > 1)
                        {
                            Songs[i].Name = EncoreCount + EncoreCount.GetOrdinalSuffix() + " ENCORE";
                        }
                        else
                        {
                            Songs[i].Name = "ENCORE";
                        }
                    }
                    Songs[i].DisplayColor = breakColor;
                }
            }
        }

        public void MoveSongUp(SetlistSong song)
        {
            var index = Songs.IndexOf(song);
            if (index <= 0) return;

            Songs[index] = Songs[index - 1];
            Songs[index - 1] = song;
            AdjustSongs();
        }

        public void MoveSongDown(SetlistSong song) 
        {
            int index = Songs.IndexOf(song);
            if (index >= Songs.Count - 1) return;

            Songs[index] = Songs[index + 1];
            Songs[index + 1] = song;
            AdjustSongs();
        }

        public void RemoveSong(SetlistSong song)
        {
            Songs.Remove(song);
            AdjustSongs();
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

        public void AddIntermission()
        {
            Songs.Add(new SetlistSong("INTERMISSION", 0, breakColor, SetlistItemType.Intermission));
            AdjustSongs();
        }

        public void AddEncore()
        {
            Songs.Add(new SetlistSong("ENCORE", 0, breakColor, SetlistItemType.Encore));
            EncoreCount++;
            AdjustSongs();
        }

        public void Save()
        {
            mainWindowVm.fileReading.SaveSetlist(Setlist, Songs.ToList());
            mainWindowVm.OpenSetlistSelect(Artist);
        }

        public async void Cancel()
        {
            if (HasChanged)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Do you want to leave without saving? Changes will be lost.", ButtonEnum.YesNo);

                var result = await box.ShowAsync();
                if (result.ToString() == "No") return;
            }

            mainWindowVm.OpenSetlistSelect(Artist);
        }
    }
}
