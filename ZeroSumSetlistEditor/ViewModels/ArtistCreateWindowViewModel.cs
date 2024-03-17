using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class ArtistCreateWindowViewModel : ViewModelBase
    {
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public string TitleText { get; }
        public string EditingArtist { get; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public ArtistCreateWindowViewModel(string artist)
        {
            EditingArtist = artist;
            TitleText = artist == string.Empty ? "Create Artist" : "Edit Artist";
            Description = artist;
        }

        public void CreateArtist()
        {
            CloseDialog?.Invoke();
        }
    }
}
