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
            this.WhenActivated(action =>
                ViewModel!.ShowCreateRoleDialog = () => { ViewModel!.RoleEdit.ShowDialog.RegisterHandler(DoShowCreateRoleDialogAsync); });
            this.WhenActivated(action =>
                ViewModel!.ShowCreateSetlistDialog = () => { ViewModel!.SetlistSelect.ShowDialog.RegisterHandler(DoShowCreateSetlistDialogAsync); });
            this.WhenActivated(action =>
                ViewModel!.ShowSetlistAddSongDialog = () => { ViewModel!.SetlistEdit.ShowDialog.RegisterHandler(DoShowSetlistAddSongDialogAsync); });
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

        private async Task DoShowCreateRoleDialogAsync(InteractionContext<CreateWindowViewModel, RoleEditViewModel?> interaction)
        {
            var dialog = new CreateWindow(((MainWindowViewModel)DataContext!).RoleEdit);
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<RoleEditViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowCreateSetlistDialogAsync(InteractionContext<SetlistCreateWindowViewModel, SetlistSelectViewModel?> interaction)
        {
            var dialog = new SetlistCreateWindow(((MainWindowViewModel)DataContext!).SetlistSelect);
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SetlistSelectViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowSetlistAddSongDialogAsync(InteractionContext<SetlistAddSongWindowViewModel, SetlistEditViewModel?> interaction)
        {
            var dialog = new SetlistAddSongWindow(((MainWindowViewModel)DataContext!).SetlistEdit);
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SetlistEditViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}