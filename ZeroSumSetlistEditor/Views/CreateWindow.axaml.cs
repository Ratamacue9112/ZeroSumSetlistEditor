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

namespace ZeroSumSetlistEditor;

public partial class CreateWindow : Window
{
    private ArtistSelectViewModel? artistSelectVm;
    private SongSelectViewModel? songSelectVm;

    public CreateWindow(ArtistSelectViewModel artistSelectVm)
    {
        this.artistSelectVm = artistSelectVm;

        Opened += BindCloseDialog;
        InitializeComponent();
    }

    public CreateWindow(SongSelectViewModel songSelectVm)
    {
        this.songSelectVm = songSelectVm;

        Opened += BindCloseDialog;
        InitializeComponent();
    }

    private void BindCloseDialog(object sender, EventArgs e)
    {
        var vm = (CreateWindowViewModel)DataContext!;
        vm.CloseDialog += () =>
        {
            string error = "";
            if (string.IsNullOrEmpty(vm.Description))
            {
                error = "No name has been entered.";
            }

            string path;
            if (vm.CreateWindowMode == CreateWindowMode.CreateArtist || vm.CreateWindowMode == CreateWindowMode.EditArtist)
            {
                path = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.Description;
                if (Directory.Exists(path))
                {
                    error = "Artist already exists.";
                }
            }
            else
            {
                path = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.EditingArtist + Path.DirectorySeparatorChar + vm.EditingArtist + "_Songs.csv";
            }

            if (string.IsNullOrEmpty(error))
            {
                try
                {
                    if (vm.CreateWindowMode == CreateWindowMode.CreateArtist)
                    {
                        Directory.CreateDirectory(path);
                        artistSelectVm!.Artists.Add(vm.Description);
                        artistSelectVm!.Artists.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditArtist)
                    {
                        string oldPath = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.EditingArtist;
                        Directory.Move(oldPath, path);
                        artistSelectVm!.Artists.Remove(vm.EditingArtist);
                        artistSelectVm!.Artists.Add(vm.Description);
                        artistSelectVm!.Artists.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.CreateSong)
                    {
                        string text = vm.Description;
                        for (int i = 0; i < vm.RoleCount; i++)
                        {
                            text += ",";
                        }
                        File.AppendAllText(path, Environment.NewLine + text);
                        songSelectVm!.Songs.Add(new Song(vm.Description, new List<string>()));
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditSong)
                    {

                    }
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