using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.ViewModels;

namespace ZeroSumSetlistEditor.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(action =>
                action(ViewModel!.ArtistSelect.ShowDialog.RegisterHandler(DoShowCreateArtistDialogAsync)));
            this.WhenActivated(action => 
                ViewModel!.ShowCreateSongDialog = () => { ViewModel!.SongSelect.ShowDialog.RegisterHandler(DoShowCreateSongDialogAsync); });
        }

        private async Task DoShowCreateArtistDialogAsync(InteractionContext<CreateWindowViewModel, ArtistSelectViewModel?> interaction)
        {
            var dialog = new CreateWindow(((MainWindowViewModel)DataContext!).ArtistSelect);
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<ArtistSelectViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowCreateSongDialogAsync(InteractionContext<CreateWindowViewModel, SongSelectViewModel?> interaction)
        {
            var dialog = new CreateWindow(((MainWindowViewModel)DataContext!).SongSelect);
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SongSelectViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}