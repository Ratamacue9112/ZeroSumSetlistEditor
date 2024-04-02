using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public class SetlistSettingsViewModel : ViewModelBase
    {
        public string Artist { get; set; }
        public SetlistSettings Settings { get; set; }

        private MainWindowViewModel mainWindowVm;

        public SetlistSettingsViewModel(string artist, SetlistSettings settings, MainWindowViewModel mainWindowVm)
        {
            Artist = artist;
            Settings = settings;
            this.mainWindowVm = mainWindowVm;
        }

        public void Save()
        {
            mainWindowVm.fileReading.SaveSetlistSettings(Settings);
            mainWindowVm.OpenSetlistSelect(Artist);
        }

        public async void ResetToDefaults()
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Really reset to defaults?", ButtonEnum.YesNo);
            var result = await box.ShowAsync();
            if (result.ToString() == "No") return;

            Settings.ResetToDefaults();
            Save();
        }

        public async void Cancel()
        {
            if (Settings.HasChanged)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Warning", "Do you want to leave without saving? Changes will be lost.", ButtonEnum.YesNo);
                var result = await box.ShowAsync();
                if (result.ToString() == "No") return;
            }

            mainWindowVm.OpenSetlistSelect(Artist);
        }
    }
}
