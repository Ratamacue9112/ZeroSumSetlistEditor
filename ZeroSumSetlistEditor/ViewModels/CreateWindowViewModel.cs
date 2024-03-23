﻿using Avalonia.Controls;
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
        EditRole
    }

    public class CreateWindowViewModel : ViewModelBase
    {
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public string TitleText { get; }
        public string EditingArtist { get; }
        public string EditingSongOrRole { get; }
        public CreateWindowMode CreateWindowMode { get; }
        public int RoleCount { get; }

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
                    break;
                case CreateWindowMode.EditSong:
                    TitleText = "Rename Song";
                    Text = songOrRole;
                    break;
                case CreateWindowMode.CreateRole:
                    TitleText = "Create Role";
                    break;
                case CreateWindowMode.EditRole:
                    TitleText = "Edit Role";
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
