using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using ZeroSumSetlistEditor.ViewModels;
using System.IO;
using ZeroSumSetlistEditor.Models;
using ZeroSumSetlistEditor.DataClasses;

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
                    Directory.CreateDirectory(path);
                    artistSelectVm.Artists.Add(new Artist(vm.Description));
                    Close();
                    return;
                }
                catch
                {
                    error = "Artist couldn't be created.";
                }
            }
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", error, ButtonEnum.Ok);

            var result = box.ShowAsync();
        };
    }
}