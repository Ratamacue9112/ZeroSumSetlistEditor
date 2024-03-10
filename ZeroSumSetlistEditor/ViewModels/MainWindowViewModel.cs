using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private FileReading fileReading;

        public MainWindowViewModel()
        {
            fileReading = new FileReading();

            ArtistSelect = new ArtistSelectViewModel(fileReading.GetArtists());
        }

        public ArtistSelectViewModel ArtistSelect { get; }
    }
}
