using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

    public class StatisticTimeFrame : IComparable
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

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not StatisticTimeFrame) return 1;

            var timeFrame = (obj as StatisticTimeFrame)!;
            if (TimeFrame == "All-time") return -1;

            return TimeFrame.CompareTo(timeFrame.TimeFrame) * -1;
        }

        public static StatisticTimeFrame operator +(StatisticTimeFrame a, StatisticTimeFrame b)
        {
            var addedTimeFrame = new StatisticTimeFrame
            {
                TimeFrame = a.TimeFrame
            };

            #region Play counts
            var bCounts = b.PlayCounts;
            var addedCounts = new ObservableCollection<StatisticSong>();
            foreach (var song in a.PlayCounts)
            {
                var index = bCounts.FindSong(song.Name);
                if (index >= 0)
                {
                    song.Count += bCounts[index].Count;
                    bCounts.RemoveAt(index);
                }
                addedCounts.Add(song);
            }
            foreach (var song in bCounts)
            {
                addedCounts.Add(song);
            }
            addedTimeFrame.PlayCounts = addedCounts;
            #endregion

            #region Show openers
            bCounts = b.ShowOpeners;
            addedCounts = new ObservableCollection<StatisticSong>();
            foreach (var song in a.ShowOpeners)
            {
                var index = bCounts.FindSong(song.Name);
                if (index >= 0)
                {
                    song.Count += bCounts[index].Count;
                    bCounts.RemoveAt(index);
                }
                addedCounts.Add(song);
            }
            foreach (var song in bCounts)
            {
                addedCounts.Add(song);
            }
            addedTimeFrame.ShowOpeners = addedCounts;
            #endregion

            #region Main set closers
            bCounts = b.MainSetClosers;
            addedCounts = new ObservableCollection<StatisticSong>();
            foreach (var song in a.MainSetClosers)
            {
                var index = bCounts.FindSong(song.Name);
                if (index >= 0)
                {
                    song.Count += bCounts[index].Count;
                    bCounts.RemoveAt(index);
                }
                addedCounts.Add(song);
            }
            foreach (var song in bCounts)
            {
                addedCounts.Add(song);
            }
            addedTimeFrame.MainSetClosers = addedCounts;
            #endregion

            #region Show closers
            bCounts = b.ShowClosers;
            addedCounts = new ObservableCollection<StatisticSong>();
            foreach (var song in a.ShowClosers)
            {
                var index = bCounts.FindSong(song.Name);
                if (index >= 0)
                {
                    song.Count += bCounts[index].Count;
                    bCounts.RemoveAt(index);
                }
                addedCounts.Add(song);
            }
            foreach (var song in bCounts)
            {
                addedCounts.Add(song);
            }
            addedTimeFrame.ShowClosers = addedCounts;
            #endregion

            var addedOtherStats = a.OtherStats;
            addedOtherStats[0].Value += b.OtherStats[0].Value;
            addedOtherStats[1].Value += b.OtherStats[1].Value;
            addedOtherStats[2].Value = addedTimeFrame.PlayCounts.Count;
            addedTimeFrame.OtherStats = addedOtherStats;

            return addedTimeFrame;
        }
    }
}
