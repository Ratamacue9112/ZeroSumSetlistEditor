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
    private SetlistEditViewModel? setlistEditVm;

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

    public CreateWindow(SetlistEditViewModel setlistEditVm)
    {
        this.setlistEditVm = setlistEditVm;

        Opened += BindCloseDialog;
        InitializeComponent();
    }

    private void BindCloseDialog(object sender, EventArgs e)
    {
        var vm = (CreateWindowViewModel)DataContext!;
        vm.CloseDialog += () =>
        {
            string error = "";
            if (vm.CreateWindowMode != CreateWindowMode.EditOneOffNote && string.IsNullOrEmpty(vm.Text))
            {
                error = "No name has been entered.";
            }

            string path;
            if (vm.CreateWindowMode == CreateWindowMode.CreateArtist || vm.CreateWindowMode == CreateWindowMode.EditArtist)
            {
                path = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.Text;
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
                        string csvPath = path + Path.DirectorySeparatorChar + vm.Text + "_Songs.csv";
                        File.WriteAllText(csvPath, "songs");
                        artistSelectVm!.Artists.Add(vm.Text);
                        artistSelectVm!.Artists.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditArtist)
                    {
                        string oldPath = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + vm.EditingArtist;
                        Directory.Move(oldPath, path);
                        artistSelectVm!.Artists.Remove(vm.EditingArtist);
                        artistSelectVm!.Artists.Add(vm.Text);
                        artistSelectVm!.Artists.Sort();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.CreateSong)
                    {
                        string text = vm.Text;
                        List<string> notes = new List<string>();
                        for (int i = 0; i < vm.RoleCount; i++)
                        {
                            text += ",";
                            notes.Add("");
                        }
                        File.AppendAllText(path, Environment.NewLine + text);
                        songSelectVm!.Songs.Add(new Song(vm.Text, vm.AltText, notes, vm.EditingArtist));
                        songSelectVm!.Songs.Sort();
                        songSelectVm!.FilterSongs();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditSong)
                    {
                        vm.mainWindowVm!.fileReading.RenameSong(vm.EditingSongOrRole, vm.Text, vm.AltText, vm.EditingArtist);

                        foreach (Song song in songSelectVm!.Songs)
                        {
                            if (song.Name == vm.EditingSongOrRole)
                            {
                                song.Name = vm.Text;
                                song.ShortName = vm.AltText;
                            }
                        }
                        songSelectVm!.Songs.Sort();
                        songSelectVm!.FilterSongs();
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.CreateRole)
                    {
                        vm.mainWindowVm!.fileReading.AddRole(vm.EditingArtist, vm.Text);

                        roleEditVm!.Roles.Add(vm.Text);
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditRole)
                    {
                        vm.mainWindowVm!.fileReading.RenameRole(vm.EditingArtist, vm.EditingSongOrRole, vm.Text);

                        roleEditVm!.Roles.Remove(vm.EditingSongOrRole);
                        roleEditVm!.Roles.Add(vm.Text);
                    }
                    else if (vm.CreateWindowMode == CreateWindowMode.EditOneOffNote)
                    {
                        for (int i = 0; i < setlistEditVm!.Songs.Count; i++)
                        {
                            if (setlistEditVm!.Songs[i] == setlistEditVm!.CurrentEditingSong)
                            {
                                setlistEditVm!.Songs[i].OneOffNote = vm.Text;
                                setlistEditVm!.Songs[i].OneOffNoteDisplay = vm.Text == "" ? "" : ("(" + vm.Text + ")");
                                setlistEditVm!.HasChanged = true;
                                break;
                            }
                        } 
                    }
                    Close();
                    return;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex);
                    error = "An error occurred.";
                }
            }
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", error, ButtonEnum.Ok);

            var result = box.ShowAsync();
        };
    }
}