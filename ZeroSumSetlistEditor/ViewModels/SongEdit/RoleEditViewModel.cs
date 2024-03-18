using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;
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

        public async void RemoveRole(string role)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Delete Role",
                ContentMessage = "Really delete role \"" + role + "\"? All associated notes will be deleted and cannot be recovered.",
                ButtonDefinitions = new List<ButtonDefinition>() {
                    new ButtonDefinition { Name = "Yes" },
                    new ButtonDefinition { Name = "No" }
                }
            });

            var result = await box.ShowAsync();
            if (result == "Yes")
            {
                mainWindowVm!.fileReading.RemoveRole(Artist, role);
                Roles.Remove(role);
                Roles.Sort();
            }
        }
    }
}
