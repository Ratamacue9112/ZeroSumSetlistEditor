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

        public SetlistAddSongWindow(SetlistEditViewModel setlistEditVm)
        {
            this.setlistEditVm = setlistEditVm;

            Opened += BindCloseDialog;
            InitializeComponent();
        }

        private void BindCloseDialog(object sender, EventArgs e)
        {
            var vm = (SetlistAddSongWindowViewModel)DataContext!;
            vm.CloseDialog += () =>
            {
                setlistEditVm.SongCount++;
                setlistEditVm.Songs.Add(new SetlistSong(vm.SelectedSong, setlistEditVm.SongCount, setlistEditVm.GetDisplayColor(setlistEditVm.SongCount), SetlistItemType.Song));
                Close();
            };
        }
    }
}