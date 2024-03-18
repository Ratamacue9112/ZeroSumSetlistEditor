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
    public enum CreateWindowMode
    {
        CreateArtist,
        EditArtist,
        CreateSong,
        EditSong,
    }

    public class CreateWindowViewModel : ViewModelBase
    {
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public string TitleText { get; }
        public string EditingArtist { get; }
        public string EditingSong { get; }
        public CreateWindowMode CreateWindowMode { get; }
        public int RoleCount { get; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public MainWindowViewModel? mainWindowVm;

        public CreateWindowViewModel(string artist, CreateWindowMode createWindowMode, string song = "", int roleCount = 0, MainWindowViewModel? mainWindowVm = null)
        {
            EditingArtist = artist;
            EditingSong = song;
            CreateWindowMode = createWindowMode;
            RoleCount = roleCount;
            this.mainWindowVm = mainWindowVm;
            switch (createWindowMode)
            {
                case CreateWindowMode.CreateArtist:
                    TitleText = "Create Artist";
                    break;
                case CreateWindowMode.EditArtist:
                    TitleText = "Edit Artist";
                    Description = artist;
                    break;
                case CreateWindowMode.CreateSong:
                    TitleText = "Create Song";
                    break;
                case CreateWindowMode.EditSong:
                    TitleText = "Rename Song";
                    Description = song;
                    break;
            }
        }

        public void Create()
        {
            CloseDialog?.Invoke();
        }
    }
}
