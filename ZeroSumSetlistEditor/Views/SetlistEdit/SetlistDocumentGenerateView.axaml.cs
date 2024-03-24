using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ZeroSumSetlistEditor.ViewModels;

namespace ZeroSumSetlistEditor.Views
{
    public partial class SetlistDocumentGenerateView : UserControl
    {
        public SetlistDocumentGenerateView()
        {
            InitializeComponent();
            Initialized += BindOpenSaveDialog;
        }

        private void BindOpenSaveDialog(object? sender, EventArgs e)
        {
            var vm = (SetlistDocumentGenerateViewModel)DataContext!;
            vm.OpenSaveDialog += async (document, setlist) =>
            {
                var topLevel = TopLevel.GetTopLevel(this);

                var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Document",
                    DefaultExtension = "pdf",
                    SuggestedFileName = setlist.Artist + "_" + setlist.Venue + "_" + setlist.Date.ToString("yyyy-MM-dd"),
                    FileTypeChoices = new List<FilePickerFileType>() { 
                        FilePickerFileTypes.Pdf
                    }
                });

                if (file != null)
                {
                    var path = file.TryGetLocalPath()!.ToString();
                    await File.WriteAllBytesAsync(path, document);
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = path,
                        FileName = "explorer.exe"
                    };
                    Process.Start(startInfo);
                }
            };
        }
    }
}