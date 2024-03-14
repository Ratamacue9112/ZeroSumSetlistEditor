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
                action(ViewModel!.ArtistSelect.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        }

        private async Task DoShowDialogAsync(InteractionContext<ArtistCreateWindowViewModel, ArtistSelectViewModel?> interaction)
        {
            var dialog = new ArtistCreateWindow(((MainWindowViewModel)DataContext!).ArtistSelect);
            dialog.DataContext = new ArtistCreateWindowViewModel();

            var result = await dialog.ShowDialog<ArtistSelectViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}