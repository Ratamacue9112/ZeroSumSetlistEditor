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
    public class FileReading
    {
        public readonly static string PersistentDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZeroSumSetlistEditor");

        public int GetSongListVersion(string artist)
        {
            string path = Path.Combine(PersistentDataPath, artist);
            if (!Directory.Exists(path))
            {
                return 2;
            }

            string csvPath = Path.Combine(path, artist + "_Songs.csv");
            if (!File.Exists(csvPath))
            {
                File.Create(csvPath);
                return 2;
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
            List<string> lines = ["version,2"];

            string headers = "songs,shortnames";
            foreach (string role in GetRoles(artist))
            {
                headers += "," + role;
            }
            lines.Add(headers);

            foreach (Song song in songs)
            {
                string line = song.Name + "," + song.ShortName;
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
                File.Create(csvPath);
                return new List<Song>();
            }

            List<Song> songs = new List<Song>();

            using TextFieldParser parser = new TextFieldParser(csvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            if (GetSongListVersion(artist) > 1)
            {
                parser.ReadFields();
            }
            int rowCount = 0;
            while (!parser.EndOfData)
            {
                string songName = "";
                string shortSongName = "";
                List<string> notes = new List<string>();

                string[] fields = parser.ReadFields()!;
                int fieldCount = 0;
                foreach (string field in fields)
                {
                    if (rowCount > 0 && fieldCount == 0)
                    {
                        songName = field;
                    }
                    else if (GetSongListVersion(artist) >= 2 && rowCount > 0 && fieldCount == 1)
                    {
                        shortSongName = field;
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
                    songs.Add(new Song(songName, shortSongName, notes, artist)); 
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
                return new Song("", "", new List<string>(), "");
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

        public void RenameSong(string song, string newName, string newShortName, string artist)
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

            int version = GetSongListVersion(artist);

            string[] fields = parser.ReadFields()!;
            if (version > 1)
            {
                fields = parser.ReadFields()!;
            }
            int fieldCount = 0;
            foreach (string field in fields)
            {
                if (fieldCount > (version > 1 ? 1 : 0))
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
                filename = filename.Split(".").First();
                string[] filenameSplit = filename.Split(" == ");
                Setlist setlist = new Setlist(filenameSplit.Last(), DateTime.ParseExact(filenameSplit.First(), "yyyy-MM-dd", CultureInfo.InvariantCulture), artist);
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
                        songs.Add(new Song(song, "", new List<string>(), setlist.Artist));
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
                switch (lineSplit.First())
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
    }
}
