using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Views;

namespace ZeroSumSetlistEditor.ViewModels 
{
    public class RoleEditViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public Interaction<CreateWindowViewModel, RoleEditViewModel?> ShowDialog { get; }

        public MainWindowViewModel mainWindowVm;

        public RoleEditViewModel(string artist, List<string> roles, MainWindowViewModel mainWindowVm) 
        {
            Artist = artist;
            Roles = new ObservableCollection<string>(roles);
            Roles.Sort();
            ShowDialog = new Interaction<CreateWindowViewModel, RoleEditViewModel?>();
            this.mainWindowVm = mainWindowVm;
        }

        public async void OpenCreateRoleDialog(string role)
        {
            var window = new CreateWindowViewModel(Artist, role == "" ? CreateWindowMode.CreateRole : CreateWindowMode.EditRole, role, Roles.Count, mainWindowVm);
            var result = await ShowDialog.Handle(window);
        }
    }
}
