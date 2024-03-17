using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

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
            List<string> roles = new List<string>();

            using TextFieldParser parser = new TextFieldParser(csvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            int rowCount = 0;
            while (!parser.EndOfData)
            {
                string songName = "";
                Dictionary<string, string> notes = new Dictionary<string, string>();

                string[] fields = parser.ReadFields()!;
                int fieldCount = 0;
                foreach (string field in fields)
                {
                    if (rowCount == 0 && fieldCount > 0)
                    {
                        roles.Add(field);
                    }
                    else if (rowCount > 0 && fieldCount == 0)
                    {
                        songName = field;
                    }
                    else if (rowCount > 0 && fieldCount > 0)
                    {
                        notes.Add(roles[fieldCount - 1], field);
                    }
                    fieldCount++;
                }
                rowCount++;

                if (songName != string.Empty) 
                { 
                    songs.Add(new Song(songName, notes)); 
                }
            }
            return songs;
        }
    }
}
