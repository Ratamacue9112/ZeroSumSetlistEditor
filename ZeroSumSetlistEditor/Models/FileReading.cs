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
using System.Collections.ObjectModel;

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

        public int GetSongListVersion(string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return 3;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.Create(csvPath);
                return 3;
            }

            using TextFieldParser parser = new TextFieldParser(csvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            string[] fields = parser.ReadFields()!;
            
            if (fields[0] == "version")
            {
                return int.Parse(fields[1]);
            }
            else
            {
                return 1;
            }
        }

        public void UpdateSongList(string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.Create(csvPath);
                return;
            }

            List<Song> songs = GetSongs(artist);
            List<string> lines = ["version,3"];

            string headers = "songs,shortnames,minutes,seconds";
            foreach (string role in GetRoles(artist))
            {
                headers += "," + role;
            }
            lines.Add(headers);

            foreach (Song song in songs)
            {
                string line = song.Name + "," + song.ShortName + "," + song.Minutes + "," + song.Seconds;
                foreach (string note in song.Notes)
                {
                    line += "," + note;
                }
                lines.Add(line);
            }

            File.WriteAllLines(csvPath, lines);
        }

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
            var version = GetSongListVersion(artist);
            if (version >= 2)
            {
                parser.ReadFields();
                parser.ReadFields();
            }
            int rowCount = 0;
            while (!parser.EndOfData)
            {
                string songName = "";
                string shortSongName = "";
                int minutes = 0;
                int seconds = 0;
                List<string> notes = new List<string>();

                string[] fields = parser.ReadFields()!;
                int fieldCount = 0;
                foreach (string field in fields)
                {
                    if (fieldCount == 0)
                    {
                        songName = field;
                    }
                    else if (version >= 2 && fieldCount == 1)
                    {
                        shortSongName = field;
                    }
                    else if (version >= 3 && fieldCount == 2)
                    {
                        int.TryParse(field, out minutes);
                    }
                    else if (version >= 3 && fieldCount == 3)
                    {
                        int.TryParse(field, out seconds);
                    }
                    else if (fieldCount > 0)
                    {
                        notes.Add(field);
                    }
                    fieldCount++;
                }
                rowCount++;

                if (songName != string.Empty) 
                { 
                    songs.Add(new Song(songName, shortSongName, minutes, seconds, notes, artist)); 
                }
            }
            songs.Sort();
            return songs;
        }
    
        public Song GetSong(string songName, string artist)
        {
            var songs = GetSongs(artist);
            var index = GetIndexOfSong(songName, songs);
            if (index >= 0)
            {
                return songs[index];
            }
            else
            {
                return new Song("", "", 0, 0, new List<string>(), "");
            }
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

        public void RenameSong(string song, string newName, string newShortName, int newMinutes, int newSeconds, string artist)
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
                else if (i == 1)
                {
                    newLine += "," + newShortName;
                }
                else if (i == 2)
                {
                    newLine += "," + newMinutes;
                }
                else if (i == 3)
                {
                    newLine += "," + newSeconds;
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

            var version = GetSongListVersion(artist);

            if (version >= 2)
            {
                parser.ReadFields();
            }
            string[] fields = parser.ReadFields()!;
            int fieldCount = 0;
            int minimumField = 0;
            if (version >= 2) minimumField = 1;
            if (version >= 3) minimumField = 3;
            foreach (string field in fields)
            {
                if (fieldCount > minimumField)
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
                File.WriteAllText(csvPath, "version,2\nsongs,shortnames," + role);
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            for (int i = 1; i < lines.Length; i++)
            {
                if (i == 1)
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
                File.WriteAllText(csvPath, "version,2\n,songs,shortnames");
                return;
            }

            List<string> lines = File.ReadAllLines(csvPath).ToList();
            List<string> newLines = [];
            foreach (string line in lines)
            {
                string[] notes = line.Split(",");
                string newLine = "";
                for (int i = 1; i < notes.Length - 1; i++)
                {
                    if (i == 1)
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
                File.WriteAllText(csvPath, "version,2\nsongs,shortnames," + newName);
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            string[] lineSplit = lines[0].Split(",");
            string newLine = "";
            for (int i = 1; i < lineSplit.Length; i++)
            {
                if (i == 1)
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
            string newLine = song + "," + GetSong(song, artist).ShortName + ",,";
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

        public List<KeyValuePair<string, string>> GetSetlistSongsWithNotes(Setlist setlist)
        {
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            if (File.Exists(path))
            {
                List<KeyValuePair<string, string>> songs = new List<KeyValuePair<string, string>>();
                var encoreCount = 0;
                foreach (string song in File.ReadAllLines(path))
                {
                    if (string.IsNullOrEmpty(song)) continue;
                    var songSplit = song.Split("!(note)");
                    if (songSplit[0] == "--ENCORE--")
                    {
                        encoreCount++;
                        if (encoreCount >= 2) songSplit[0] = "--ENCORE " + encoreCount + "--";
                    }
                    songs.Add(new KeyValuePair<string, string>(songSplit[0], songSplit.Length > 1 ? songSplit.Last() : ""));
                }
                return songs;
            }
            return new List<KeyValuePair<string, string>>();
        }

        public int GetIndexOfSong(string song, List<Song> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == song) return i;
            }
            return -1;
        }

        public Dictionary<Song, string> GetSetlistSongsFullDetail(Setlist setlist)
        {
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            if (File.Exists(path))
            {
                var songs = new Dictionary<Song, string>();
                var allSongs = GetSongs(setlist.Artist);
                foreach (KeyValuePair<string, string> song in GetSetlistSongsWithNotes(setlist))
                {
                    if (string.IsNullOrEmpty(song.Key)) continue;
                    if (song.Key.StartsWith("--") && song.Key.EndsWith("--"))
                    {
                        songs.Add(new Song(song.Key, "", 0, 0, new List<string>(), setlist.Artist), song.Value);
                    }
                    foreach (Song s in allSongs)
                    {
                        if (s.Name == song.Key) songs.Add(s, song.Value);
                    }
                }
                return songs;
            }
            return new Dictionary<Song, string>();
        }

        public void SaveSetlist(Setlist setlist, List<SetlistSong> songs, List<SetlistChange> changes)
        {
            // Manage statistics
            var stats = GetStatistics(setlist.Artist);
            // Check if statistics are empty, if so, rescan
            if (stats.Count <= 1)
            {
                stats = RescanStatistics(setlist.Artist);
            }

            int yearIndex = -1;
            for (int i = 0; i < stats.Count; i++)
            {
                if (stats[i].TimeFrame == setlist.Date.Year.ToString())
                {
                    yearIndex = i;
                    break;
                }
            }
            if (yearIndex == -1)
            {
                yearIndex = stats.Count;
                stats.Add(new StatisticTimeFrame { 
                    TimeFrame = setlist.Date.Year.ToString(),
                    OtherStats = new ObservableCollection<OtherStat>
                    {
                        new OtherStat("Shows played", "shows", 0),
                        new OtherStat("Total songs played", "songs", 0),
                        new OtherStat("Unique songs played", "songs", 0)
                    }
                });
            }

            foreach (var c in changes)
            {
                // Process add/delete changes
                if (c is SetlistAddDeleteChange)
                {
                    var change = (c as SetlistAddDeleteChange)!;
                    if (change.SongName == "ENCORE" || change.SongName == "INTERMISSION") continue;
                    // Console.WriteLine("Add/Delete - " + change.SongName + ", " + change.Deleted);

                    var index = stats[0].PlayCounts.FindSong(change.SongName);
                    if (index >= 0)
                    {
                        stats[0].PlayCounts[index].Count += change.Deleted ? -1 : 1;
                        if (stats[0].PlayCounts[index].Count < 1)
                        {
                            stats[0].PlayCounts.RemoveAt(index);
                        }
                    }
                    else if (!change.Deleted) stats[0].PlayCounts.Add(new StatisticSong(change.SongName, 1));
                    stats[0].OtherStats[1].Value += change.Deleted ? -1 : 1;

                    index = stats[yearIndex].PlayCounts.FindSong(change.SongName);
                    if (index >= 0)
                    {
                        stats[yearIndex].PlayCounts[index].Count += change.Deleted ? -1 : 1;
                        if (stats[yearIndex].PlayCounts[index].Count < 1)
                        {
                            stats[yearIndex].PlayCounts.RemoveAt(index);
                        }
                        stats[yearIndex].OtherStats[1].Value += change.Deleted ? -1 : 1;
                    }
                    else if (!change.Deleted) stats[yearIndex].PlayCounts.Add(new StatisticSong(change.SongName, 1));
                    stats[yearIndex].OtherStats[1].Value += change.Deleted ? -1 : 1;
                }
                // Process main position changes
                else if (c is SetlistMainPositionChange)
                {
                    var change = (c as SetlistMainPositionChange)!;
                    var found = 0;
                    // Console.WriteLine("Position - " + change.OldSongName + " : " + change.NewSongName + ", " + change.Position.ToString());
                    switch (change.Position)
                    {
                        case SetlistMainPosition.ShowOpener:
                            foreach (var song in stats[0].ShowOpeners)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[0].ShowOpeners.Add(new StatisticSong(change.NewSongName, 1));
                            foreach (var song in stats[yearIndex].ShowOpeners)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[yearIndex].ShowOpeners.Add(new StatisticSong(change.NewSongName, 1));
                            break;
                        case SetlistMainPosition.MainSetCloser:
                            foreach (var song in stats[0].MainSetClosers)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[0].MainSetClosers.Add(new StatisticSong(change.NewSongName, 1));
                            foreach (var song in stats[yearIndex].MainSetClosers)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[yearIndex].MainSetClosers.Add(new StatisticSong(change.NewSongName, 1));
                            break;
                        case SetlistMainPosition.ShowCloser:
                            foreach (var song in stats[0].ShowClosers)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[0].ShowClosers.Add(new StatisticSong(change.NewSongName, 1));
                            foreach (var song in stats[yearIndex].ShowClosers)
                            {
                                if (song.Name == change.OldSongName)
                                {
                                    song.Count--;
                                    found++;
                                    if (found >= 2) break;
                                }
                                if (song.Name == change.NewSongName)
                                {
                                    song.Count++;
                                    found++;
                                    if (found >= 2) break;
                                }
                            }
                            if (found < 2 && (change.NewSongName != "ENCORE" && change.NewSongName != "INTERMISSION")) stats[yearIndex].ShowClosers.Add(new StatisticSong(change.NewSongName, 1));
                            break;
                    }
                }
                // Process empty state changes
                else if (c is SetlistEmptyStateChange)
                {
                    var change = (c as SetlistEmptyStateChange)!;
                    stats[0].OtherStats[0].Value += change.Empty ? -1 : 1;
                    stats[yearIndex].OtherStats[0].Value += change.Empty ? -1 : 1;
                }
            }

            stats[0].OtherStats[2].Value = stats[0].PlayCounts.Count;
            stats[yearIndex].OtherStats[2].Value = stats[yearIndex].PlayCounts.Count;

            SaveStatistics(setlist.Artist, stats);

            // Save to file
            var path = Path.Combine(PersistentDataPath, setlist.Artist, "Setlists", setlist.Date.ToString("yyyy-MM-dd") + " == " + setlist.Venue + ".txt");
            List<string> songNames = new List<string>();
            foreach (var song in songs)
            {
                string name = "";
                switch (song.Type) 
                {
                    case SetlistItemType.Song:
                        name = song.Name;
                        break;
                    case SetlistItemType.Intermission:
                        name = "--" + song.Name + "--";
                        break;
                    case SetlistItemType.Encore:
                        name = "--ENCORE--";
                        break;
                }
                if (song.OneOffNote != "")
                {
                    name += "!(note)" + song.OneOffNote;
                }
                songNames.Add(name);
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
    
        public List<StatisticTimeFrame> GetStatistics(string artist, bool deleted = false)
        {
            var path = Path.Combine(PersistentDataPath, artist, deleted ? "deleted_stats.txt" : "stats.txt");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                return new List<StatisticTimeFrame> {
                    new StatisticTimeFrame { 
                        TimeFrame = "All-time",
                        OtherStats = new ObservableCollection<OtherStat>
                        {
                            new OtherStat("Shows played"),
                            new OtherStat("Total songs played"),
                            new OtherStat("Unique songs played")
                        }
                    }
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
                    if (currentStatistic != null)
                    {
                        if (currentStatistic.OtherStats.Count < 1)
                        {
                            currentStatistic.OtherStats.Add(new OtherStat("Shows played"));
                            currentStatistic.OtherStats.Add(new OtherStat("Total songs played"));
                            currentStatistic.OtherStats.Add(new OtherStat("Unique songs played"));
                        }
                        currentStatistic.Sort();
                        statistics.Add(currentStatistic);
                    }
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
                else if (line.StartsWith("::") && line.EndsWith("::"))
                {
                    if (currentStatistic == null) continue;
                    var lineSplit = line.Replace("::", "").Split(",");
                    if (lineSplit.Length < 3) continue;

                    var isNumber = int.TryParse(lineSplit[0], out var value);
                    if (isNumber)
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Shows played", "shows", value));
                    }
                    else
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Shows played"));
                    }

                    isNumber = int.TryParse(lineSplit[1], out value);
                    if (isNumber)
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Total songs played", "songs", value));
                    }
                    else
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Total songs played"));
                    }

                    isNumber = int.TryParse(lineSplit[2], out value);
                    if (isNumber)
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Unique songs played", "songs", value));
                    }
                    else
                    {
                        currentStatistic.OtherStats.Add(new OtherStat("Unique songs played"));
                    }
                }
                else
                {
                    if (currentStatistic == null) continue;

                    var lineSplit = line.Split(" --- ");
                    var countIsNumber = int.TryParse(lineSplit.Last(), out int count);
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
                if (currentStatistic.OtherStats.Count < 1)
                {
                    currentStatistic.OtherStats.Add(new OtherStat("Shows played"));
                    currentStatistic.OtherStats.Add(new OtherStat("Total songs played"));
                    currentStatistic.OtherStats.Add(new OtherStat("Unique songs played"));
                }
                currentStatistic.Sort();
                statistics.Add(currentStatistic);
            }
            if (statistics.Count < 1)
            {
                statistics = new List<StatisticTimeFrame> {
                    new StatisticTimeFrame { 
                        TimeFrame = "All-time",
                        OtherStats = new ObservableCollection<OtherStat>
                        {
                            new OtherStat("Shows played"),
                            new OtherStat("Total songs played"),
                            new OtherStat("Unique songs played")
                        } 
                    }
                };
            }

            return statistics;
        }
    
        public void SaveStatistics(string artist, List<StatisticTimeFrame> statistics, bool deleted = false)
        {
            List<string> lines = new List<string>();

            foreach (StatisticTimeFrame stat in statistics)
            {
                lines.Add("--" + stat.TimeFrame + "--");
                lines.Add("::" + stat.OtherStats[0].Value + ", " + stat.OtherStats[1].Value + ", " + stat.OtherStats[2].Value + "::");

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

            var path = Path.Combine(PersistentDataPath, artist, deleted ? "deleted_stats.txt" : "stats.txt");
            File.WriteAllLines(path, lines.ToArray());
        }
    
        public List<StatisticTimeFrame> RescanStatistics(string artist)
        {
            List<StatisticTimeFrame> statistics = [
                new StatisticTimeFrame { 
                    TimeFrame = "All-time",
                    OtherStats = new ObservableCollection<OtherStat> {
                        new OtherStat("Shows played", "shows", 0),
                        new OtherStat("Total songs played", "songs", 0),
                        new OtherStat("Unique songs played", "songs", 0)
                    }
                }
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
                    statistics.Add(new StatisticTimeFrame { 
                        TimeFrame = setlist.Date.Year.ToString(),
                        OtherStats = new ObservableCollection<OtherStat> {
                            new OtherStat("Shows played", "shows", 0),
                            new OtherStat("Total songs played", "songs", 0),
                            new OtherStat("Unique songs played", "songs", 0)
                        }
                    });
                }

                // Add to show count
                statistics[0].OtherStats[0].Value++;
                statistics[yearIndex].OtherStats[0].Value++;

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
                    statistics[yearIndex].OtherStats[1].Value++;
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
                    statistics[0].OtherStats[1].Value++;
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
                if (!songs[0].StartsWith("--") && !songs[0].EndsWith("--"))
                {
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
                }

                // Main set closer
                if (!songs[mainSetCloserIndex].StartsWith("--") && !songs[mainSetCloserIndex].EndsWith("--"))
                {
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
                }

                // Show closer
                if (!songs.Last().StartsWith("--") && !songs.Last().EndsWith("--"))
                {
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
                }

                currentIndex++;
            }
            foreach (var stat in statistics)
            {
                stat.OtherStats[2].Value = stat.PlayCounts.Count;
                stat.Sort();
            }

            SaveStatistics(artist, statistics);
            statistics = statistics.Combine(GetStatistics(artist, true));
            return statistics;
        }
    }
}
