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
    public class StatisticsViewModel : ViewModelBase
    {
        public string _selectedItem = "All-time";
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                UpdateCurrentTimeFrame(value);
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }

        public string Artist { get; set; }

        public ObservableCollection<StatisticTimeFrame> _statistics;
        public ObservableCollection<StatisticTimeFrame> Statistics
        {
            get => _statistics;
            set => this.RaiseAndSetIfChanged(ref _statistics, value);
        }

        public ObservableCollection<string> _timeFrames;
        public ObservableCollection<string> TimeFrames
        {
            get => _timeFrames;
            set => this.RaiseAndSetIfChanged(ref _timeFrames, value);
        }

        public StatisticTimeFrame _currentTimeFrame;
        public StatisticTimeFrame CurrentTimeFrame
        {
            get => _currentTimeFrame;
            set => this.RaiseAndSetIfChanged(ref _currentTimeFrame, value);
        }

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
