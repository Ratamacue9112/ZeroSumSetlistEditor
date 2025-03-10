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
                
                setlistEditVm.Changes.Add(new SetlistAddDeleteChange(vm.SelectedSong, false));
                if (setlistEditVm.Songs.Count < 1)
                {
                    setlistEditVm.Changes.Add(new SetlistEmptyStateChange(false));
                }

                var song = mainWindowViewModel.fileReading.GetSong(vm.SelectedSong, setlistEditVm.Artist);
                if (song.ShortName == "")
                {
                    song.ShortName = vm.SelectedSong;
                }
                setlistEditVm.Songs.Add(new SetlistSong(vm.SelectedSong, song.ShortName, song.Minutes, song.Seconds, setlistEditVm.SongCount, setlistEditVm.GetDisplayColor(setlistEditVm.SongCount), SetlistItemType.Song, ""));

                Close();
                setlistEditVm.RecalculateTime();
            };
        }
    }
}