using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using ZeroSumSetlistEditor.ViewModels;
using System.IO;
using ZeroSumSetlistEditor.Models;
using System.Collections.Generic;

namespace ZeroSumSetlistEditor
{
    public partial class SetlistAddSongWindow : Window
    {
        public SetlistEditViewModel setlistEditVm;

        private MainWindowViewModel mainWindowViewModel;

        public SetlistAddSongWindow(SetlistEditViewModel setlistEditVm, MainWindowViewModel mainWindowViewModel)
        {
            this.setlistEditVm = setlistEditVm;
            this.mainWindowViewModel = mainWindowViewModel;

            Opened += BindCloseDialog;
            InitializeComponent();
        }

        private void BindCloseDialog(object sender, EventArgs e)
        {
            var vm = (SetlistAddSongWindowViewModel)DataContext!;
            vm.CloseDialog += () =>
            {
                setlistEditVm.SongCount++;
                setlistEditVm.HasChanged = true;
                var shortName = mainWindowViewModel.fileReading.GetSong(vm.SelectedSong, setlistEditVm.Artist).ShortName;
                if (shortName == "")
                {
                    shortName = vm.SelectedSong;
                }
                setlistEditVm.Songs.Add(new SetlistSong(vm.SelectedSong, shortName, setlistEditVm.SongCount, setlistEditVm.GetDisplayColor(setlistEditVm.SongCount), SetlistItemType.Song));
                Close();
            };
        }
    }
}