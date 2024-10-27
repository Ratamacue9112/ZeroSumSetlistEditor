using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
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
        CreateRole,
        EditRole,
        EditOneOffNote
    }

    public partial class CreateWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _text = string.Empty;

        [ObservableProperty]
        private string _altText = string.Empty;

        public string TitleText { get; }
        public string EditingArtist { get; }
        public string EditingSongOrRole { get; }
        public CreateWindowMode CreateWindowMode { get; }
        public int RoleCount { get; }

        [ObservableProperty]
        private bool _showAltText;

        public string AltTextTitle { get; set; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public MainWindowViewModel? mainWindowVm;

        public CreateWindowViewModel(string artist, CreateWindowMode createWindowMode, string songOrRole = "", int roleCount = 0, MainWindowViewModel? mainWindowVm = null)
        {
            EditingArtist = artist;
            EditingSongOrRole = songOrRole;
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
                    Text = artist;
                    break;
                case CreateWindowMode.CreateSong:
                    TitleText = "Create Song";
                    ShowAltText = true;
                    AltTextTitle = "Shortened title (will show up on printed setlist)\nLeave blank to be the same as the title";
                    break;
                case CreateWindowMode.EditSong:
                    TitleText = "Rename Song";
                    Text = songOrRole;
                    AltText = mainWindowVm!.fileReading.GetSong(songOrRole, artist).ShortName;
                    ShowAltText = true;
                    AltTextTitle = "Shortened title (will show up on printed setlist)\nLeave blank to be the same as the title";
                    break;
                case CreateWindowMode.CreateRole:
                    TitleText = "Create Role";
                    break;
                case CreateWindowMode.EditRole:
                    TitleText = "Edit Role";
                    Text = songOrRole;
                    break;
                case CreateWindowMode.EditOneOffNote:
                    TitleText = "Edit One Off Note";
                    Text = songOrRole;
                    break;
            }
        }

        public void Create()
        {
            CloseDialog?.Invoke();
        }
    }
}
