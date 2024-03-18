using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using DynamicData;

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
                File.Create(csvPath);
                return new List<string>(0);
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
    }
}
