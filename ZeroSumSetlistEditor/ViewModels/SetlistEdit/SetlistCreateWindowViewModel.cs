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
        private string _venue = string.Empty;
        public string Venue
        {
            get => _venue;
            set => this.RaiseAndSetIfChanged(ref _venue, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

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
