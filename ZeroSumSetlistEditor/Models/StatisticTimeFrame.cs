using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroSumSetlistEditor.Models
{
    public class StatisticSong : IComparable
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public StatisticSong(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not StatisticSong) return 1;

            return Count.CompareTo((obj as StatisticSong)!.Count) * -1;
        }
    }

    public class OtherStat
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public int _value = 0;
        public int Value 
        {
            get 
            {
                return _value;
            }
            set
            {
                _value = value;
                ValueText = value.ToString();
            }
        }
        public string ValueText { get; set; }
        public bool CanBeFound { get; set; }

        public OtherStat(string name, string unit, int value)
        {
            Name = name;
            Unit = unit;
            Value = value;
            CanBeFound = true;
        }

        public OtherStat(string name)
        {
            Name = name;
            Unit = "";
            ValueText = "RESCAN TO VIEW";
            CanBeFound = false;
        }
    }

    public class StatisticTimeFrame
    {
        public string TimeFrame { get; set; }

        public ObservableCollection<StatisticSong> PlayCounts { get; set; }
        public ObservableCollection<StatisticSong> ShowOpeners { get; set; }
        public ObservableCollection<StatisticSong> MainSetClosers { get; set; }
        public ObservableCollection<StatisticSong> ShowClosers { get; set; }
        public ObservableCollection<OtherStat> OtherStats { get; set; }

        public StatisticTimeFrame() 
        {
            TimeFrame = "";
            PlayCounts = new ObservableCollection<StatisticSong>();
            ShowOpeners = new ObservableCollection<StatisticSong>();
            MainSetClosers = new ObservableCollection<StatisticSong>();
            ShowClosers = new ObservableCollection<StatisticSong>();
            OtherStats = new ObservableCollection<OtherStat>();
        }

        public void Sort()
        {
            PlayCounts.Sort();
            ShowOpeners.Sort();
            MainSetClosers.Sort();
            ShowClosers.Sort();
        }
    }
}
