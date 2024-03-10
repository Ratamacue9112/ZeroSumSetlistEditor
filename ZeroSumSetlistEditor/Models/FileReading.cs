using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroSumSetlistEditor.DataClasses;

namespace ZeroSumSetlistEditor.Models
{
    public class FileReading
    {
        public string PersistentDataPath { get; set; }

        public FileReading()
        {
            PersistentDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "ZeroSumSetlistEditor";
        }

        public List<Artist> GetArtists()
        {
            if (!Directory.Exists(PersistentDataPath))
            {
                Directory.CreateDirectory(PersistentDataPath);
            }

            string[] artistFolders = Directory.GetDirectories(PersistentDataPath);
            List<Artist> artists = new List<Artist>();
            foreach (string folder in artistFolders)
            {
                artists.Add(new Artist(folder.Split(Path.DirectorySeparatorChar).Last()));
            }
            return artists;
        }
    }
}
