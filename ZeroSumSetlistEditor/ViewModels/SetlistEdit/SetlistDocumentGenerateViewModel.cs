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
        public Dictionary<Song, string> Songs { get; set; }
        public bool PrintGeneral { get; set; }

        public Action<byte[], Setlist> OpenSaveDialog { get; set; }

        private MainWindowViewModel mainWindowVm;

        public SetlistDocumentGenerateViewModel(Setlist setlist, List<string> roles, Dictionary<Song, string> songs, MainWindowViewModel mainWindowVm)
        {
            Setlist = setlist;
            var list = new List<SetlistDocumentRole>();
            foreach (var role in roles)
            {
                list.Add(new SetlistDocumentRole(role));
            }
            Roles = new ObservableCollection<SetlistDocumentRole>(list);
            Songs = songs;
            PrintGeneral = false;
            this.mainWindowVm = mainWindowVm;
        }

        public void GenerateDocument()
        {
            var document = mainWindowVm.documentGeneration.GenerateDocument(Roles.ToList(), PrintGeneral, Songs, Setlist, mainWindowVm.fileReading.GetSetlistSettings());
            OpenSaveDialog(document, Setlist);
        }
    }
}
