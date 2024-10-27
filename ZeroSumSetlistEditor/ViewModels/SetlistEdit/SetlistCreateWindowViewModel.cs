using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public partial class SetlistCreateWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _venue = string.Empty;

        [ObservableProperty]
        private DateTime _date;

        public string TitleText { get; }
        public Setlist? Setlist { get; }

        public delegate void CloseDialogAction();
        public event CloseDialogAction? CloseDialog;

        public SetlistCreateWindowViewModel(Setlist? setlist)
        {
            if (setlist == null)
            {
                TitleText = "Create Setlist";
                Date = DateTime.Today;
            }
            else
            {
                TitleText = "Rename Setlist";
                Venue = setlist.Venue;
                Date = setlist.Date;
            }
            Setlist = setlist;
        }

        public void Create()
        {
            CloseDialog?.Invoke();
        }
    }
}
