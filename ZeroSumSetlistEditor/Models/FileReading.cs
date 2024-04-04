using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Globalization;
using ZeroSumSetlistEditor.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;

namespace ZeroSumSetlistEditor.Models
{
    public enum StatisticMode
    {
        None,
        AllSongs,
        ShowOpeners,
        MainSetClosers,
        ShowClosers
    }

    public class FileReading
    {
        public readonly static string PersistentDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZeroSumSetlistEditor");

        public List<string> GetArtists()
        {
            if (!Directory.Exists(PersistentDataPath))
            {
                Directory.CreateDirectory(PersistentDataPath);
            }

            string[] artistFolders = Directory.GetDirectories(PersistentDataPath);
            List<string> artists = new List<string>();
            foreach (string folder in artistFolders)
            {
                artists.Add(folder.Split(Path.DirectorySeparatorChar).Last());
            }
            return artists;
        }

        public List<Song> GetSongs(string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return new List<Song>();
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.Create(csvPath).Close();
                return new List<Song>();
            }

            List<Song> songs = new List<Song>();

            using TextFieldParser parser = new TextFieldParser(csvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            int rowCount = 0;
            while (!parser.EndOfData)
            {
                string songName = "";
                List<string> notes = new List<string>();

                string[] fields = parser.ReadFields()!;
                int fieldCount = 0;
                foreach (string field in fields)
                {
                    if (rowCount > 0 && fieldCount == 0)
                    {
                        songName = field;
                    }
                    else if (rowCount > 0 && fieldCount > 0)
                    {
                        notes.Add(field);
                    }
                    fieldCount++;
                }
                rowCount++;

                if (songName != string.Empty) 
                { 
                    songs.Add(new Song(songName, notes, artist)); 
                }
            }
            songs.Sort();
            return songs;
        }
    
        public void RemoveSong(string song, string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist, artist + "_Songs.csv");
            List<string> linesList = File.ReadAllLines(path).ToList();
            int index = 0;
            foreach (string line in linesList)
            {
                if (line.StartsWith(song))
                {
                    break;
                }
                index++;
            }
            linesList.RemoveAt(index);
            File.WriteAllLines(path, linesList);
        }

        public void RenameSong(string song, string newName, string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist, artist + "_Songs.csv");
            List<string> linesList = File.ReadAllLines(path).ToList();
            int index = 0;
            foreach (string line in linesList)
            {
                if (line.StartsWith(song))
                {
                    break;
                }
                index++;
            }
            string newLine = "";
            string[] splitLine = linesList[index].Split(",");
            for (int i = 0; i < splitLine.Length; i++)
            {
                if (i == 0)
                {
                    newLine += newName;
                }
                else
                {
                    newLine += "," + splitLine[i];
                }
            }
            linesList[index] = newLine;
            File.WriteAllLines(path, linesList);
        }

        public List<string> GetRoles(string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.WriteAllText(csvPath, "songs");
                return new List<string>();
            }

            List<string> roles = new List<string>();

            using TextFieldParser parser = new TextFieldParser(csvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            string[] fields = parser.ReadFields()!;
            int fieldCount = 0;
            foreach (string field in fields)
            {
                if (fieldCount > 0)
                {
                    if (field != string.Empty)
                    {
                        roles.Add(field);
                    }
                }
                fieldCount++;
            }

            return roles;
        }
    
        public void AddRole(string artist, string role)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.WriteAllText(csvPath, "songs," + role);
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    lines[i] += "," + role;
                }
                else
                {
                    lines[i] += ",";
                }
            }
            File.WriteAllLines(csvPath, lines);

            return;
        }

        public void RemoveRole(string artist, string role)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.WriteAllText(csvPath, "songs");
                return;
            }

            List<string> lines = File.ReadAllLines(csvPath).ToList();
            List<string> newLines = [];
            foreach (string line in lines)
            {
                string[] notes = line.Split(",");
                string newLine = "";
                for (int i = 0; i < notes.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        newLine += notes[i];
                    }
                    else
                    {
                        newLine += "," + notes[i];
                    }
                }
                newLines.Add(newLine);
            }
            File.WriteAllLines(csvPath, newLines);

            return;
        }

        public void RenameRole(string artist, string role, string newName)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.WriteAllText(csvPath, "songs," + newName);
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            string[] lineSplit = lines[0].Split(",");
            string newLine = "";
            for (int i = 0; i < lineSplit.Length; i++)
            {
                if (i == 0)
                {
                    newLine += lineSplit[i];
                }
                else if (lineSplit[i] == role)
                {
                    newLine += "," + newName;
                }
                else
                {
                    newLine += "," + lineSplit[i];
                }
            }
            lines[0] = newLine;
            File.WriteAllLines(csvPath, lines);

            return;
        }
    
        public void SaveNotes(string artist, string song, List<SongNote> notes)
        {
            string path = Path.Combine(PersistentDataPath, artist, artist + "_Songs.csv");
            List<string> linesList = File.ReadAllLines(path).ToList();
            int index = 0;
            foreach (string line in linesList)
            {
                if (line.StartsWith(song))
                {
                    break;
                }
                index++;
            }
            string newLine = song;
            foreach (SongNote note in notes)
            {
                newLine += "," + note.Note;
            }
            linesList[index] = newLine;
            File.WriteAllLines(path, linesList);
        }
    
        public List<Setlist> GetSetlists(string artist)
        {
            var path = Path.Combine(PersistentDataPath, artist, "Setlists");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            List<Setlist> list = new List<Setlist>();
            foreach(string file in Directory.GetFiles(path))
            {
                if (!file.EndsWith(".txt")) continue;
                string filename = file.Split(Path.DirectorySeparatorChar).Last();
                filename = filename.Split(".")[0];
                string[] filenameSplit = filename.Split(" == ");
                Setlist setlist = new Setlist(filenameSplit.Last(), DateTime.ParseExact(filenameSplit[0], "yyyy-MM-dd", CultureInfo.InvariantCulture), artist);
                list.Add(setlist);
            }
            list.Sort();
            return list;
        }

        public List<string> GetSetlistSongs(Setlist setlist)
        {
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            if (File.Exists(path))
            {
                List<string> songs = new List<string>();
                foreach(string song in File.ReadAllLines(path))
                {
                    if (string.IsNullOrEmpty(song)) continue;
                    songs.Add(song);
                }
                return songs;
            }
            return new List<string>();
        }

        public int GetIndexOfSong(string song, List<Song> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == song) return i;
            }
            return -1;
        }

        public List<Song> GetSetlistSongsFullDetail(Setlist setlist)
        {
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            if (File.Exists(path))
            {
                var songs = new List<Song>();
                var allSongs = GetSongs(setlist.Artist);
                foreach (string song in File.ReadAllLines(path))
                {
                    if (string.IsNullOrEmpty(song)) continue;
                    if (song.StartsWith("--") && song.EndsWith("--"))
                    {
                        songs.Add(new Song(song, new List<string>(), setlist.Artist));
                    }
                    foreach (Song s in allSongs)
                    {
                        if (s.Name == song) songs.Add(s);
                    }
                }
                return songs;
            }
            return new List<Song>();
        }

        public void SaveSetlist(Setlist setlist, List<SetlistSong> songs)
        {
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            List<string> songNames = new List<string>();
            foreach (var song in songs)
            {
                switch (song.Type) 
                { 
                    case SetlistItemType.Song:
                        songNames.Add(song.Name);
                        break;
                    case SetlistItemType.Intermission:
                        songNames.Add("--" + song.Name + "--");
                        break;
                    case SetlistItemType.Encore:
                        songNames.Add("--ENCORE--");
                        break;
                }
            }
            File.WriteAllLines(path, songNames);
        }
    
        public void SaveSetlistSettings(SetlistSettings settings)
        {
            var path = Path.Combine(PersistentDataPath, "setlist_settings.txt");

            List<string> text = [
                "backgroundColor - " + settings.BackgroundColor.ToString(),
                "headerColor - " + settings.HeaderColor.ToString(),
                "songColor - " + settings.SongColor.ToString(),
                "noteColor - " + settings.NoteColor.ToString(),
                "intermissionColor - " + settings.IntermissionColor.ToString(),
                "encoreColor - " + settings.EncoreColor.ToString(),
                "fontFamily - " + settings.FontFamily,
                "headerSize - " + settings.HeaderSize.ToString(),
                "songSize - " + settings.SongSize.ToString(),
                "noteSize - " + settings.NoteSize.ToString(),
                "intermissionSize - " + settings.IntermissionSize.ToString(),
                "encoreSize - " + settings.EncoreSize.ToString(),
                "showVenue - " + settings.ShowVenue.ToString(),
                "showDate - " + settings.ShowDate.ToString(),
                "showArtist - " + settings.ShowArtist.ToString(),
            ];
            File.WriteAllLines(path, text);
        }

        public SetlistSettings GetSetlistSettings()
        {
            var path = Path.Combine(PersistentDataPath, "setlist_settings.txt");
            SetlistSettings settings = new SetlistSettings();
            settings.Initialized = false;
            if (!File.Exists(path))
            {
                settings.ResetToDefaults();
                return settings;
            }
            
            foreach (var line in File.ReadAllLines(path))
            {
                var lineSplit = line.Split(" - ");
                switch (lineSplit[0])
                {
                    case "backgroundColor":
                        var isColor = Color.TryParse(lineSplit.Last(), out var color);
                        if (isColor)
                        {
                            settings.BackgroundColor = color;
                        }
                        else
                        {
                            settings.BackgroundColor = Colors.White;
                        }
                        break;
                    case "headerColor":
                        isColor = Color.TryParse(lineSplit.Last(), out color);
                        if (isColor)
                        {
                            settings.HeaderColor = color;
                        }
                        else
                        {
                            settings.HeaderColor = Colors.Black;
                        }
                        break;
                    case "songColor":
                        isColor = Color.TryParse(lineSplit.Last(), out color);
                        if (isColor)
                        {
                            settings.SongColor = color;
                        }
                        else
                        {
                            settings.SongColor = Colors.Black;
                        }
                        break;
                    case "noteColor":
                        isColor = Color.TryParse(lineSplit.Last(), out color);
                        if (isColor)
                        {
                            settings.NoteColor = color;
                        }
                        else
                        {
                            settings.NoteColor = Colors.Black;
                        }
                        break;
                    case "intermissionColor":
                        isColor = Color.TryParse(lineSplit.Last(), out color);
                        if (isColor)
                        {
                            settings.IntermissionColor = color;
                        }
                        else
                        {
                            settings.IntermissionColor = Colors.Black;
                        }
                        break;
                    case "encoreColor":
                        isColor = Color.TryParse(lineSplit.Last(), out color);
                        if (isColor)
                        {
                            settings.EncoreColor = color;
                        }
                        else
                        {
                            settings.EncoreColor = Colors.Black;
                        }
                        break;
                    case "fontFamily":
                        settings.FontFamily = lineSplit.Last();
                        break;
                    case "headerSize":
                        var isInt = int.TryParse(lineSplit.Last(), out var result);
                        if (isInt)
                        {
                            settings.HeaderSize = result;
                        }
                        else
                        {
                            settings.HeaderSize = 20;
                        }
                        break;
                    case "songSize":
                        isInt = int.TryParse(lineSplit.Last(), out result);
                        if (isInt)
                        {
                            settings.SongSize = result;
                        }
                        else
                        {
                            settings.SongSize = 20;
                        }
                        break;
                    case "noteSize":
                        isInt = int.TryParse(lineSplit.Last(), out result);
                        if (isInt)
                        {
                            settings.NoteSize = result;
                        }
                        else
                        {
                            settings.NoteSize = 20;
                        }
                        break;
                    case "intermissionSize":
                        isInt = int.TryParse(lineSplit.Last(), out result);
                        if (isInt)
                        {
                            settings.IntermissionSize = result;
                        }
                        else
                        {
                            settings.IntermissionSize = 20;
                        }
                        break;
                    case "encoreSize":
                        isInt = int.TryParse(lineSplit.Last(), out result);
                        if (isInt)
                        {
                            settings.EncoreSize = result;
                        }
                        else
                        {
                            settings.EncoreSize = 20;
                        }
                        break;
                    case "showVenue":
                        var isBool = bool.TryParse(lineSplit.Last(), out var value);
                        if (isBool)
                        {
                            settings.ShowVenue = value;
                        }
                        else
                        {
                            settings.ShowVenue = true;
                        }
                        break;
                    case "showDate":
                        isBool = bool.TryParse(lineSplit.Last(), out value);
                        if (isBool)
                        {
                            settings.ShowDate = value;
                        }
                        else
                        {
                            settings.ShowDate = true;
                        }
                        break;
                    case "showArtist":
                        isBool = bool.TryParse(lineSplit.Last(), out value);
                        if (isBool)
                        {
                            settings.ShowArtist = value;
                        }
                        else
                        {
                            settings.ShowArtist = false;
                        }
                        break;
                }
            }
            settings.Initialized = true;
            return settings;
        }
    
        public List<StatisticTimeFrame> GetStatistics(string artist)
        {
            var path = Path.Combine(PersistentDataPath, artist, "stats.txt");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                return new List<StatisticTimeFrame> {
                    new StatisticTimeFrame { TimeFrame = "All-time" }
                };
            }

            List<StatisticTimeFrame> statistics = new List<StatisticTimeFrame>();
            StatisticTimeFrame? currentStatistic = null;
            StatisticMode currentStatisticMode = StatisticMode.None;

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) 
                {
                    continue;
                }
                else if (line.StartsWith("--") && line.EndsWith("--")) 
                {
                    if (currentStatistic != null) statistics.Add(currentStatistic);
                    currentStatistic = new StatisticTimeFrame { TimeFrame = line.Replace("--", "") };
                }
                else if (line.StartsWith("**") && line.EndsWith("**"))
                {
                    switch(line.Replace("**", ""))
                    {
                        case "all-songs":
                            currentStatisticMode = StatisticMode.AllSongs;
                            break;
                        case "show-openers":
                            currentStatisticMode = StatisticMode.ShowOpeners;
                            break;
                        case "main-set-closers":
                            currentStatisticMode = StatisticMode.MainSetClosers;
                            break;
                        case "show-closers":
                            currentStatisticMode = StatisticMode.ShowClosers;
                            break;
                    }
                }
                else
                {
                    if (currentStatistic == null) continue;

                    var lineSplit = line.Split(" --- ");
                    var name = lineSplit[0];
                    var count = 0;
                    var countIsNumber = int.TryParse(lineSplit.Last(), out count);
                    if (!countIsNumber) continue;
                    switch (currentStatisticMode)
                    {
                        case StatisticMode.AllSongs:
                            currentStatistic.PlayCounts.Add(new StatisticSong(lineSplit[0], count));
                            break;
                        case StatisticMode.ShowOpeners:
                            currentStatistic.ShowOpeners.Add(new StatisticSong(lineSplit[0], count));
                            break;
                        case StatisticMode.MainSetClosers:
                            currentStatistic.MainSetClosers.Add(new StatisticSong(lineSplit[0], count));
                            break;
                        case StatisticMode.ShowClosers:
                            currentStatistic.ShowClosers.Add(new StatisticSong(lineSplit[0], count));
                            break;
                    }
                }
            }
            if (currentStatistic != null)
            {
                statistics.Add(currentStatistic);
            }
            if (statistics.Count < 1)
            {
                statistics = new List<StatisticTimeFrame> {
                    new StatisticTimeFrame { TimeFrame = "All-time" }
                };
            }

            return statistics;
        }
    
        public void SaveStatistics(string artist, List<StatisticTimeFrame> statistics)
        {
            List<string> lines = new List<string>();

            foreach (StatisticTimeFrame stat in statistics)
            {
                lines.Add("--" + stat.TimeFrame + "--");

                lines.Add("**all-songs**");
                foreach (StatisticSong song in stat.PlayCounts)
                {
                    lines.Add(song.Name + " --- " + song.Count);
                }

                lines.Add("**show-openers**");
                foreach (StatisticSong song in stat.ShowOpeners)
                {
                    lines.Add(song.Name + " --- " + song.Count);
                }

                lines.Add("**main-set-closers**");
                foreach (StatisticSong song in stat.MainSetClosers)
                {
                    lines.Add(song.Name + " --- " + song.Count);
                }

                lines.Add("**show-closers**");
                foreach (StatisticSong song in stat.ShowClosers)
                {
                    lines.Add(song.Name + " --- " + song.Count);
                }
            }

            var path = Path.Combine(PersistentDataPath, artist, "stats.txt");
            File.WriteAllLines(path, lines.ToArray());
        }
    
        public List<StatisticTimeFrame> RescanStatistics(string artist)
        {
            List<StatisticTimeFrame> statistics = [
                new StatisticTimeFrame { TimeFrame = "All-time" }
            ];

            foreach (var setlist in GetSetlists(artist))
            {
                var songs = GetSetlistSongs(setlist);

                if (songs.Count <= 0) continue;

                // Get year
                int yearIndex = -1;
                for (int i = 0; i < statistics.Count; i++)
                {
                    if (statistics[i].TimeFrame == setlist.Date.Year.ToString())
                    {
                        yearIndex = i;
                        break;
                    }
                }
                if (yearIndex == -1)
                {
                    yearIndex = statistics.Count;
                    statistics.Add(new StatisticTimeFrame { TimeFrame = setlist.Date.Year.ToString() });
                }

                int currentIndex = 0;
                int mainSetCloserIndex = -1;
                int songIndex = -1;

                // Play counts
                foreach (string song in songs)
                {
                    if (song.StartsWith("--") && song.EndsWith("--"))
                    {
                        if (mainSetCloserIndex == -1 && song == "--ENCORE--")
                        {
                            mainSetCloserIndex = yearIndex - 1;
                        }
                        continue;
                    }

                    // Add to year
                    songIndex = statistics[yearIndex].PlayCounts.FindSong(song);
                    if (songIndex >= 0)
                    {
                        statistics[yearIndex].PlayCounts[songIndex].Count++;
                    }
                    else
                    {
                        statistics[yearIndex].PlayCounts.Add(new StatisticSong(song, 1));  
                    }

                    // Add to all-time
                    songIndex = statistics[0].PlayCounts.FindSong(song);
                    if (songIndex >= 0)
                    {
                        statistics[0].PlayCounts[songIndex].Count++;
                    }
                    else
                    {
                        statistics[0].PlayCounts.Add(new StatisticSong(song, 1));
                    }
                }
                // Set main set closer index if no encore
                if (mainSetCloserIndex == -1)
                {
                    mainSetCloserIndex = songs.Count - 1;
                }

                // Show opener
                songIndex = statistics[yearIndex].ShowOpeners.FindSong(songs[0]);
                if (songIndex >= 0)
                {
                    statistics[yearIndex].ShowOpeners[songIndex].Count++;
                }
                else
                {
                    statistics[yearIndex].ShowOpeners.Add(new StatisticSong(songs[0], 1));
                }

                songIndex = statistics[0].ShowOpeners.FindSong(songs[0]);
                if (songIndex >= 0)
                {
                    statistics[0].ShowOpeners[songIndex].Count++;
                }
                else
                {
                    statistics[0].ShowOpeners.Add(new StatisticSong(songs[0], 1));
                }

                // Main set closer
                songIndex = statistics[yearIndex].MainSetClosers.FindSong(songs[mainSetCloserIndex]);
                if (songIndex >= 0)
                {
                    statistics[yearIndex].MainSetClosers[songIndex].Count++;
                }
                else
                {
                    statistics[yearIndex].MainSetClosers.Add(new StatisticSong(songs[mainSetCloserIndex], 1));
                }

                songIndex = statistics[0].MainSetClosers.FindSong(songs[mainSetCloserIndex]);
                if (songIndex >= 0)
                {
                    statistics[0].MainSetClosers[songIndex].Count++;
                }
                else
                {
                    statistics[0].MainSetClosers.Add(new StatisticSong(songs[mainSetCloserIndex], 1));
                }

                // Show closer
                songIndex = statistics[yearIndex].ShowClosers.FindSong(songs.Last());
                if (songIndex >= 0)
                {
                    statistics[yearIndex].ShowClosers[songIndex].Count++;
                }
                else
                {
                    statistics[yearIndex].ShowClosers.Add(new StatisticSong(songs.Last(), 1));
                }

                songIndex = statistics[0].ShowClosers.FindSong(songs.Last());
                if (songIndex >= 0)
                {
                    statistics[0].ShowClosers[songIndex].Count++;
                }
                else
                {
                    statistics[0].ShowClosers.Add(new StatisticSong(songs.Last(), 1));
                }

                currentIndex++;
            }

            SaveStatistics(artist, statistics);
            return statistics;
        }
    }
}
