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

    public class StatisticTimeFrame
    {
        public string TimeFrame { get; set; }
        public ObservableCollection<StatisticSong> PlayCounts { get; set; }
        public ObservableCollection<StatisticSong> ShowOpeners { get; set; }
        public ObservableCollection<StatisticSong> MainSetClosers { get; set; }
        public ObservableCollection<StatisticSong> ShowClosers { get; set; }

        public StatisticTimeFrame() 
        {
            TimeFrame = "";
            PlayCounts = new ObservableCollection<StatisticSong>();
            ShowOpeners = new ObservableCollection<StatisticSong>();
            MainSetClosers = new ObservableCollection<StatisticSong>();
            ShowClosers = new ObservableCollection<StatisticSong>();
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
