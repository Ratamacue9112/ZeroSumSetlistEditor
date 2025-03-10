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
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ZeroSumSetlistEditor.ViewModels
{
    public enum SetlistItemType
    {
        Song,
        Intermission,
        Encore
    }

    public partial class SetlistEditViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<SetlistSong> _songs = new ObservableCollection<SetlistSong>();
        [ObservableProperty]
        private int _totalHours = 0;
        [ObservableProperty]
        private int _totalMinutes = 0;
        [ObservableProperty]
        private int _totalSeconds = 0;
        [ObservableProperty]
        private string _totalTimeDisplay = "";

        public int SongCount { get; set; }
        public int EncoreCount { get; set; }

        public Setlist Setlist { get; set; }
        public string Artist { get; set; }

        public SetlistSong CurrentEditingSong { get; set; }

        public bool HasChanged { get; set; }
        public List<SetlistChange> Changes { get; set; }

        public Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?> ShowAddSongDialog { get; }
        public Interaction<CreateWindowViewModel, SetlistEditViewModel?> ShowEditOneOffNoteDialog { get; }

        private List<SetlistSong> startingSongs;
        private MainWindowViewModel mainWindowVm;

        private const string breakColor = "#0D777C";

        public SetlistEditViewModel(Setlist setlist, List<KeyValuePair<string, string>> songs, MainWindowViewModel mainWindowVm)
        {
            SongCount = 0;
            EncoreCount = 0;
            var list = new List<SetlistSong>();
            foreach (KeyValuePair<string, string> song in songs)
            {
                if (song.Key.StartsWith("--") && song.Key.EndsWith("--"))
                {
                    var name = song.Key.Replace("--", "");
                    var encore = name.StartsWith("ENCORE");
                    SetlistSong setlistSong = new SetlistSong(name, name, encore ? 3 : 15, 0, -1, breakColor, encore ? SetlistItemType.Encore : SetlistItemType.Intermission, song.Value);
                    if (setlistSong.Type == SetlistItemType.Encore)
                    {
                        EncoreCount++;
                        if (EncoreCount > 1)
                        {
                            setlistSong.Name = "ENCORE " + EncoreCount;
                        }
                    }
                    list.Add(setlistSong);
                }
                else
                {
                    SongCount++;
                    var songObj = mainWindowVm.fileReading.GetSong(song.Key, setlist.Artist);
                    if (songObj.ShortName == "")
                    {
                        songObj.ShortName = song.Key;
                    }
                    list.Add(new SetlistSong(song.Key, songObj.ShortName, songObj.Minutes, songObj.Seconds, SongCount, GetDisplayColor(SongCount), SetlistItemType.Song, song.Value));
                }
            }

            Songs = new ObservableCollection<SetlistSong>(list);
            Setlist = setlist;
            Artist = setlist.Artist;
            HasChanged = false;
            Changes = new List<SetlistChange>();
            ShowAddSongDialog = new Interaction<SetlistAddSongWindowViewModel, SetlistEditViewModel?>();
            ShowEditOneOffNoteDialog = new Interaction<CreateWindowViewModel, SetlistEditViewModel?>();
            this.mainWindowVm = mainWindowVm;
            startingSongs = list;

            RecalculateTime();
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
                            Songs[i].Name = "ENCORE " + EncoreCount;
                        }
                        else
                        {
                            Songs[i].Name = "ENCORE";
                        }
                        Songs[i].DisplayColor = breakColor;
                    }
                }
            }
        }

        public void RecalculateTime()
        {
            TotalHours = 0;
            TotalMinutes = 0;
            TotalSeconds = 0;
            foreach(SetlistSong song in Songs)
            {
                TotalMinutes += song.TimeMinutes;
                TotalSeconds += song.TimeSeconds;
            }
            var remainder = TotalMinutes % 60;
            TotalHours = (TotalMinutes - remainder) / 60;
            var minutesRemainder = TotalSeconds % 60;
            TotalMinutes = remainder + ((TotalSeconds - minutesRemainder) / 60);
            TotalSeconds = minutesRemainder;


            if (TotalHours > 0)
            {
                TotalTimeDisplay = TotalHours.ToString() + ":" + TotalMinutes.ToString("00") + ":" + TotalSeconds.ToString("00");
            }
            else
            {
                TotalTimeDisplay = TotalMinutes.ToString() + ":" + TotalSeconds.ToString("00");
            }
        }

        public async void OpenEditOneOffNoteDialog(SetlistSong song)
        {
            CurrentEditingSong = song;
            var window = new CreateWindowViewModel(Artist, CreateWindowMode.EditOneOffNote, song.OneOffNote);
            var result = await ShowEditOneOffNoteDialog.Handle(window);
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
            var index = Songs.IndexOf(song);
            Songs.Remove(song);
            Changes.Add(new SetlistAddDeleteChange(song.Name, true));
            if (Songs.Count < 1)
            {
                Changes.Add(new SetlistEmptyStateChange(true));
            }
            AdjustSongs();
            RecalculateTime();
        } 

        public string GetDisplayColor(int number)
        {
            return number % 2 == 0 ? "#6E6E6E" : "#5E5E5E";
        }

        public async void OpenAddSongDialog()
        {
            var window = new SetlistAddSongWindowViewModel(Artist, mainWindowVm.fileReading.GetSongs(Artist));
            var result = await ShowAddSongDialog.Handle(window);
        }

        public void AddIntermission()
        {
            Songs.Add(new SetlistSong("INTERMISSION", "INTERMISSION", 15, 0, 0, breakColor, SetlistItemType.Intermission, ""));
            AdjustSongs();
            RecalculateTime();
        }

        public void AddEncore()
        {
            Songs.Add(new SetlistSong("ENCORE", "ENCORE", 3, 0, 0, breakColor, SetlistItemType.Encore, ""));
            EncoreCount++;
            AdjustSongs();
            RecalculateTime();
        }

        public void Save()
        {
            // Add main position changes

            // If setlist was not empty
            if (startingSongs.Count > 0)
            {
                // If setlist is still not empty
                if (Songs.Count > 0)
                {
                    // Show opener
                    if (startingSongs[0].Name != Songs[0].Name)
                    {
                        Changes.Add(new SetlistMainPositionChange(startingSongs[0].Name, Songs[0].Name, SetlistMainPosition.ShowOpener));
                    }
                    // Show closer
                    if (startingSongs.Last().Name != Songs.Last().Name)
                    {
                        Changes.Add(new SetlistMainPositionChange(startingSongs.Last().Name, Songs.Last().Name, SetlistMainPosition.ShowCloser));
                    }

                    // Main set closer
                    // Find old index of main set closer
                    int oldMainSetCloserIndex = -1;
                    for (int i = 0; i < startingSongs.Count; i++)
                    {
                        if (startingSongs[i].Name == "ENCORE")
                        {
                            oldMainSetCloserIndex = i - 1;
                            break;
                        }
                    }
                    // Find new index of main set closer
                    int newMainSetCloserIndex = -1;
                    for (int i = 0; i < Songs.Count; i++)
                    {
                        if (Songs[i].Name == "ENCORE")
                        {
                            newMainSetCloserIndex = i - 1;
                            break;
                        }
                    }

                    // Get old and new main set closer names and compares them
                    // If they are the different, add a new change
                    var oldMainSetCloserName = startingSongs[oldMainSetCloserIndex == -1 ? startingSongs.Count - 1 : oldMainSetCloserIndex].Name;
                    var newMainSetCloserName = Songs[newMainSetCloserIndex == -1 ? Songs.Count - 1 : newMainSetCloserIndex].Name;
                    if (oldMainSetCloserName != newMainSetCloserName)
                    {
                        Changes.Add(new SetlistMainPositionChange(oldMainSetCloserName, newMainSetCloserName, SetlistMainPosition.MainSetCloser));
                    }
                }
                // If setlist is now empty
                else
                {
                    Changes.Add(new SetlistMainPositionChange(startingSongs[0].Name, "", SetlistMainPosition.ShowOpener));
                    Changes.Add(new SetlistMainPositionChange(startingSongs.Last().Name, "", SetlistMainPosition.ShowCloser));

                    int mainSetCloserIndex = -1;
                    for (int i = 0; i < startingSongs.Count; i++)
                    {
                        if (startingSongs[i].Name == "ENCORE")
                        {
                            mainSetCloserIndex = i - 1;
                            break;
                        }
                    }

                    var mainSetCloserName = startingSongs[mainSetCloserIndex == -1 ? startingSongs.Count - 1 : mainSetCloserIndex].Name;
                    Changes.Add(new SetlistMainPositionChange(mainSetCloserName, "", SetlistMainPosition.MainSetCloser));
                }
            }
            // If setlist was empty
            else
            {
                if (Songs.Count > 0)
                {
                    Changes.Add(new SetlistMainPositionChange("", Songs[0].Name, SetlistMainPosition.ShowOpener));
                    Changes.Add(new SetlistMainPositionChange("", Songs.Last().Name, SetlistMainPosition.ShowCloser));

                    int mainSetCloserIndex = -1;
                    for (int i = 0; i < Songs.Count; i++)
                    {
                        if (Songs[i].Name == "ENCORE")
                        {
                            mainSetCloserIndex = i - 1;
                            break;
                        }
                    }

                    var mainSetCloserName = Songs[mainSetCloserIndex == -1 ? Songs.Count - 1 : mainSetCloserIndex].Name;
                    Changes.Add(new SetlistMainPositionChange("", mainSetCloserName, SetlistMainPosition.MainSetCloser));
                }
            }

            mainWindowVm.fileReading.SaveSetlist(Setlist, Songs.ToList(), Changes);
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
