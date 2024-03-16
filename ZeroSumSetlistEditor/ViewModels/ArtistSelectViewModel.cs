using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using ZeroSumSetlistEditor.Models;
using System.IO;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class ArtistSelectViewModel : ViewModelBase
    {
        public ObservableCollection<string> Artists { get; set; }

        public ICommand OpenCreateArtistDialogCommand { get; }

        public Interaction<ArtistCreateWindowViewModel, ArtistSelectViewModel?> ShowDialog { get; }

        public ArtistSelectViewModel(List<string> artists) 
        { 
            Artists = new ObservableCollection<string>(artists);

            ShowDialog = new Interaction<ArtistCreateWindowViewModel, ArtistSelectViewModel?>();

            OpenCreateArtistDialogCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var window = new ArtistCreateWindowViewModel();
                var result = await ShowDialog.Handle(window);
            });
        }
    
        public async void DeleteArtist(string name)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Delete Artist",
                ContentMessage = "Really delete artist \"" + name + "\"? Song and setlist data will be deleted and cannot be recovered.",
                ButtonDefinitions = new List<ButtonDefinition>() {
                    new ButtonDefinition { Name = "Yes" },
                    new ButtonDefinition { Name = "No" }
                }
            });

            var result = await box.ShowAsync();
            if (result == "Yes")
            {
                var path = FileReading.PersistentDataPath + Path.DirectorySeparatorChar + name;
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Artists.Remove(name);
                Artists.Sort();
            }
        }
    }
}
