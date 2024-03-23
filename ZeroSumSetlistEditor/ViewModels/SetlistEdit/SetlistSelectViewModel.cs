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

        public SetlistSelectViewModel(string artist, List<Setlist> setlists)
        {
            Artist = artist;
            Setlists = new ObservableCollection<Setlist>(setlists);
            ShowDialog = new Interaction<SetlistCreateWindowViewModel, SetlistSelectViewModel?>();
        }

        public async void OpenCreateSetlistDialog(Setlist setlist)
        {
            var window = new SetlistCreateWindowViewModel(setlist);
            var result = await ShowDialog.Handle(window);
        }
    }
}
