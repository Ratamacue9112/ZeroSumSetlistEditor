using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Collections;

namespace ZeroSumSetlistEditor.Models
{
    public class FileReading
    {
        public readonly static string PersistentDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "ZeroSumSetlistEditor";

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
            string path = PersistentDataPath + Path.DirectorySeparatorChar + artist;
            if (!Directory.Exists(path))
            {
                return new List<Song>();
            }

            string csvPath = path + Path.DirectorySeparatorChar + artist + "_Songs.csv";
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
                    songs.Add(new Song(songName, notes)); 
                }
            }
            songs.Sort();
            return songs;
        }
    
        public void RemoveSong(string song, string artist)
        {
            string path = PersistentDataPath + Path.DirectorySeparatorChar + artist + Path.DirectorySeparatorChar + artist + "_Songs.csv";
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
            string path = PersistentDataPath + Path.DirectorySeparatorChar + artist + Path.DirectorySeparatorChar + artist + "_Songs.csv";
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
            string path = PersistentDataPath + Path.DirectorySeparatorChar + artist;
            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            string csvPath = path + Path.DirectorySeparatorChar + artist + "_Songs.csv";
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
            string path = PersistentDataPath + Path.DirectorySeparatorChar + artist;
            if (!Directory.Exists(path))
            {
                return;
            }

            string csvPath = path + Path.DirectorySeparatorChar + artist + "_Songs.csv";
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
    }
}
