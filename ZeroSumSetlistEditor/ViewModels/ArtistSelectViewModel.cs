using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.DataClasses;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class ArtistSelectViewModel : ViewModelBase
    {
        public ArtistSelectViewModel(List<Artist> artists) { 
            Artists = new ObservableCollection<Artist>(artists);
        }

        public ObservableCollection<Artist> Artists { get; }
    }
}
