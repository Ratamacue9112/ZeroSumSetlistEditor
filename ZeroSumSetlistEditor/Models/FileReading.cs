using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
