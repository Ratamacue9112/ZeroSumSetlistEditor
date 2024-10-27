using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.Models;

namespace ZeroSumSetlistEditor.ViewModels
{
    public partial class StatisticsViewModel : ViewModelBase
    {
        public string _selectedItem = "All-time";
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                UpdateCurrentTimeFrame(value);
                SetProperty(ref _selectedItem, value);
            }
        }

        public string Artist { get; set; }

        [ObservableProperty]
        public ObservableCollection<StatisticTimeFrame> _statistics;

        [ObservableProperty]
        public ObservableCollection<string> _timeFrames;

        [ObservableProperty]
        public StatisticTimeFrame _currentTimeFrame;

        private MainWindowViewModel mainWindowVm;

        public StatisticsViewModel(string artist, List<StatisticTimeFrame> statistics, MainWindowViewModel mainWindowVm)
        {
            Artist = artist;
            Statistics = new ObservableCollection<StatisticTimeFrame>(statistics);
            TimeFrames = new ObservableCollection<string>();
            foreach (var stat in statistics)
            {
                TimeFrames.Add(stat.TimeFrame);
            }
            CurrentTimeFrame = statistics[0];
            this.mainWindowVm = mainWindowVm;
        }

        public void UpdateCurrentTimeFrame(string timeFrame)
        {
            foreach (var stat in Statistics)
            {
                if (timeFrame == stat.TimeFrame) CurrentTimeFrame = stat;
            }
        }

        public void RescanStatistics()
        {
            Statistics = new ObservableCollection<StatisticTimeFrame>(mainWindowVm.fileReading.RescanStatistics(Artist));
            TimeFrames = new ObservableCollection<string>();
            foreach (var stat in Statistics)
            {
                TimeFrames.Add(stat.TimeFrame);
            }
            CurrentTimeFrame = Statistics[0];
            SelectedItem = "All-time";
        }
    }
}
