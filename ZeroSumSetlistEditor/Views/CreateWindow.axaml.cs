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
    private RoleEditViewModel? roleEditVm;

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

    public CreateWindow(RoleEditViewModel roleEditVm)
    {
        this.roleEditVm = roleEditVm;

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
                        string csvPath = path + Path.DirectorySeparatorChar + vm.Description + "_Songs.csv";
                        File.WriteAllText(csvPath, "songs");
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
                        songSelectVm!.Songs.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditSong)
                    {
                        vm.mainWindowVm!.fileReading.RenameSong(vm.EditingSongOrRole, vm.Description, vm.EditingArtist);

                        foreach (Song song in songSelectVm!.Songs)
                        {
                            if (song.Name == vm.EditingSongOrRole)
                            {
                                song.Name = vm.Description;
                            }
                        }
                        songSelectVm!.Songs.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.CreateRole)
                    {
                        vm.mainWindowVm!.fileReading.AddRole(vm.EditingArtist, vm.Description);

                        roleEditVm!.Roles.Add(vm.Description);
                        roleEditVm!.Roles.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditRole)
                    {
                        vm.mainWindowVm!.fileReading.RenameRole(vm.EditingArtist, vm.EditingSongOrRole, vm.Description);

                        roleEditVm!.Roles.Remove(vm.EditingSongOrRole);
                        roleEditVm!.Roles.Add(vm.Description);
                        roleEditVm!.Roles.Sort();
                    }
                    Close();
                    return;
                }
                catch
                {
                    error = "An error occured.";
                }
            }
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", error, ButtonEnum.Ok);

            var result = box.ShowAsync();
        };
    }
}