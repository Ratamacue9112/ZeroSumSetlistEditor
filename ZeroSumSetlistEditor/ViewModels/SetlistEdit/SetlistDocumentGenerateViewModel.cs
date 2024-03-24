using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistDocumentRole
    {
        public string Name { get; set; }
        public bool ToPrint { get; set; }

        public SetlistDocumentRole(string name)
        {
            Name = name;
            ToPrint = true;
        }
    }

    public class SetlistDocumentGenerateViewModel : ViewModelBase
    {
        public Setlist Setlist { get; set; }
        public ObservableCollection<SetlistDocumentRole> Roles { get; set; }
        public bool PrintGeneral { get; set; }

        public SetlistDocumentGenerateViewModel(Setlist setlist, List<string> roles)
        {
            Setlist = setlist;
            var list = new List<SetlistDocumentRole>();
            foreach (var role in roles)
            {
                list.Add(new SetlistDocumentRole(role));
            }
            Roles = new ObservableCollection<SetlistDocumentRole>(list);
            PrintGeneral = false;
        }

        public void GenerateDocument()
        {

        }
    }
}
