using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistCreateWindowViewModel : ViewModelBase
    {
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

        public string TitleText { get; }
        public bool IsEditing { get; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public SetlistCreateWindowViewModel(Setlist? setlist)
        {
            if (setlist == null)
            {
                TitleText = "Create Setlist";
                Date = DateTime.Today;
                IsEditing = false;
            }
            else
            {
                TitleText = "Rename Setlist";
                Date = setlist.Date;
                IsEditing = true;
            }
        }

        public void Create()
        {
            CloseDialog?.Invoke();
        }
    }
}
