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

namespace ZeroSumSetlistEditor.Models
{
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
                File.Create(csvPath);
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
    }
}
