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
using ZeroSumSetlistEditor.Views;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSelectViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public ObservableCollection<Setlist> Setlists { get; set; }

        public Interaction<SetlistCreateWindowViewModel, SetlistSelectViewModel?> ShowDialog { get; }

        private MainWindowViewModel mainWindowVm;

        public SetlistSelectViewModel(string artist, List<Setlist> setlists, MainWindowViewModel mainWindowVm)
        {
            Artist = artist;
            Setlists = new ObservableCollection<Setlist>(setlists);
            ShowDialog = new Interaction<SetlistCreateWindowViewModel, SetlistSelectViewModel?>();
            this.mainWindowVm = mainWindowVm;
        }

        public async void OpenCreateSetlistDialog(Setlist setlist)
        {
            var window = new SetlistCreateWindowViewModel(setlist);
            var result = await ShowDialog.Handle(window);
        }

        public async void DeleteSetlist(Setlist setlist)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Delete Setlist",
                ContentMessage = "Really delete this seltist from \"" + setlist.Venue + "\"? It cannot be recovered.",
                ButtonDefinitions = new List<ButtonDefinition>() {
                    new ButtonDefinition { Name = "Yes" },
                    new ButtonDefinition { Name = "No" }
                }
            });

            var result = await box.ShowAsync();
            if (result == "Yes")
            {
                foreach (Setlist i in Setlists)
                {
                    if (i.Venue == setlist.Venue && i.Date == setlist.Date)
                    {
                        Setlists.Remove(i);
                        break;
                    }
                }
                Setlists.Sort();

                if (mainWindowVm.fileReading.GetSetlistSongs(setlist).Count > 0)
                {
                    var stats = mainWindowVm.fileReading.GetStatistics(Artist);
                    int yearIndex = -1;
                    for (int i = 0; i < stats.Count; i++)
                    {
                        if (stats[i].TimeFrame == setlist.Date.Year.ToString())
                        {
                            yearIndex = i;
                            break;
                        }
                    }

                    stats[0].OtherStats[0].Value--;
                    if (yearIndex >= 0)
                    {
                        stats[yearIndex].OtherStats[0].Value--;
                    }

                    foreach (var song in mainWindowVm.fileReading.GetSetlistSongs(setlist))
                    {
                        stats[0].OtherStats[1].Value--;
                        var index = stats[0].PlayCounts.FindSong(song);
                        if (index >= 0)
                        {
                            stats[0].PlayCounts[index].Count--;
                            if (stats[0].PlayCounts[index].Count < 1)
                            {
                                stats[0].PlayCounts.RemoveAt(index);
                            }
                        }

                        if (yearIndex >= 0)
                        {
                            stats[yearIndex].OtherStats[1].Value--;
                            index = stats[yearIndex].PlayCounts.FindSong(song);
                            if (index >= 0)
                            {
                                stats[yearIndex].PlayCounts[index].Count--;
                                if (stats[yearIndex].PlayCounts[index].Count < 1)
                                {
                                    stats[yearIndex].PlayCounts.RemoveAt(index);
                                }
                            }
                        }
                    }

                    stats[0].OtherStats[2].Value = stats[0].PlayCounts.Count;
                    if (yearIndex >= 0)
                    {
                        stats[yearIndex].OtherStats[2].Value = stats[yearIndex].PlayCounts.Count;
                    }

                    mainWindowVm.fileReading.SaveStatistics(Artist, stats);
                }

                var path = Path.Combine(FileReading.PersistentDataPath, Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
