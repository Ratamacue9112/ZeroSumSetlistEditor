using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using ZeroSumSetlistEditor.ViewModels;
using System.IO;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor;

public partial class ArtistCreateWindow : Window
{
    private ArtistSelectViewModel artistSelectVm;

    public ArtistCreateWindow(ArtistSelectViewModel artistSelectVm)
    {
        this.artistSelectVm = artistSelectVm;

        Opened += BindCloseDialog;
        InitializeComponent();
    }

    private void BindCloseDialog(object sender, EventArgs e)
    {
        var vm = (ArtistCreateWindowViewModel)DataContext!;
        vm.CloseDialog += () =>
        {
            string path = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.Description;
            string error = "";
            if (string.IsNullOrEmpty(vm.Description)) 
            {
                error = "No name has been entered.";
            }
            else if (Directory.Exists(path))
            {
                error = "Artist already exists.";
            }

            if (string.IsNullOrEmpty(error))
            {
                try
                {
                    if (vm.EditingArtist == string.Empty)
                    {
                        Directory.CreateDirectory(path);
                    }
                    else
                    {
                        string oldPath = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.EditingArtist;
                        Directory.Move(oldPath, path);
                        artistSelectVm.Artists.Remove(vm.EditingArtist);
                    }
                    artistSelectVm.Artists.Add(vm.Description);
                    artistSelectVm.Artists.Sort();
                    Close();
                    return;
                }
                catch
                {
                    error = "Artist couldn't be created/renamed.";
                }
            }
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", error, ButtonEnum.Ok);

            var result = box.ShowAsync();
        };
    }
}