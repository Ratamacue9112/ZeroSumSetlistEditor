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

                var path = Path.Combine(FileReading.PersistentDataPath, Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
