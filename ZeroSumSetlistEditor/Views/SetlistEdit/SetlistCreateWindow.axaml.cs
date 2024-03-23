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
                if (string.IsNullOrEmpty(vm.Venue))
                {
                    error = "No venue has been entered.";
                }

                if (string.IsNullOrEmpty(error))
                {
                    try
                    {
                        var path = Path.Combine(FileReading.PersistentDataPath, setlistSelectVm.Artist, "Setlists", vm.Date.ToString("yyyy-MM-dd") + " == " + vm.Venue + ".txt");
                        if (vm.Setlist == null)
                        {
                            File.Create(path);
                            setlistSelectVm.Setlists.Add(new Setlist(vm.Venue, vm.Date, setlistSelectVm.Artist));
                        }
                        else
                        {
                            var oldPath = Path.Combine(FileReading.PersistentDataPath, setlistSelectVm.Artist, "Setlists", vm.Setlist.Date.ToString("yyyy-MM-dd") + " == " + vm.Setlist.Venue + ".txt");

                            for (int i = 0; i < setlistSelectVm.Setlists.Count; i++)
                            {
                                if (setlistSelectVm.Setlists[i].Venue == vm.Setlist.Venue && setlistSelectVm.Setlists[i].Date == vm.Setlist.Date)
                                {
                                    setlistSelectVm.Setlists[i] = new Setlist(vm.Venue, vm.Date, setlistSelectVm.Artist);
                                    break;
                                }
                            }
                            setlistSelectVm.Setlists.Sort();

                            if (File.Exists(oldPath))
                            {
                                File.Move(oldPath, path);
                            }
                        }

                        setlistSelectVm.Setlists.Sort();

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
}