using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZeroSumSetlistEditor.DataClasses;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class ArtistSelectViewModel : ViewModelBase
    {
        public ObservableCollection<Artist> Artists { get; }

        public ICommand OpenCreateArtistDialogCommand { get; }

        public Interaction<ArtistCreateWindowViewModel, ArtistSelectViewModel?> ShowDialog { get; }

        public ArtistSelectViewModel(List<Artist> artists) 
        { 
            Artists = new ObservableCollection<Artist>(artists);

            ShowDialog = new Interaction<ArtistCreateWindowViewModel, ArtistSelectViewModel?>();

            OpenCreateArtistDialogCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var window = new ArtistCreateWindowViewModel();
                var result = await ShowDialog.Handle(window);
            });
        }
    }
}
