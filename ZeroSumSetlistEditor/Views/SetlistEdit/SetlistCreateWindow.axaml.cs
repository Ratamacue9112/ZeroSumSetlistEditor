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

public partial class SetlistCreateWindow : Window
{
    public SetlistSelectViewModel setlistSelectVm;

    public SetlistCreateWindow(SetlistSelectViewModel setlistSelectVm)
    {
        this.setlistSelectVm = setlistSelectVm;

        Opened += BindCloseDialog;
        InitializeComponent();
    }

    private void BindCloseDialog(object sender, EventArgs e)
    {
        var vm = (SetlistCreateWindowViewModel)DataContext!;
        vm.CloseDialog += () =>
        {
            string error = "";
            if (string.IsNullOrEmpty(vm.Description))
            {
                error = "No venue has been entered.";
            }

            if (string.IsNullOrEmpty(error))
            {
                try
                {
                    var path = Path.Combine(FileReading.PersistentDataPath, setlistSelectVm.Artist, "Setlists", vm.Date.ToString("yyyy-MM-dd") + " == " + vm.Description + ".txt");
                    if (vm.IsEditing)
                    {

                    }
                    else
                    {
                        File.Create(path);
                        setlistSelectVm.Setlists.Add(new Setlist(vm.Description, vm.Date));
                        setlistSelectVm.Setlists.Sort();
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