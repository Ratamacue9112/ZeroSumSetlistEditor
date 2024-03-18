using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.ViewModels 
{
    public class RoleEditViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public RoleEditViewModel(string artist, List<string> roles) 
        {
            Artist = artist;
            Roles = new ObservableCollection<string>(roles);
        }
    }
}
