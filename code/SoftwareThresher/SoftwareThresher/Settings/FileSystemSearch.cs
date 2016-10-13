using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareThresher.Utilities;
using System.Text.RegularExpressions;

namespace SoftwareThresher.Settings {
   public class FileSystemSearch : Search {
      readonly ISystemFileReader systemFileReader;

      // TODO - use this
      public string BaseLocation { private get; set; }

      public FileSystemSearch() : this(new SystemFileReader()) { }

      public FileSystemSearch(ISystemFileReader systemFileReader) {
         this.systemFileReader = systemFileReader;
      }

      // TODO - Do we want to return Observations?
      public List<string> GetFiles(string directory, string searchPattern) {
         return Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
      }

      // TODO - Do we want to get an Observation?
      public List<string> GetReferencesInFile(string filename, string searchPattern) {
         try {
            var references = new List<string>();

            systemFileReader.Open(filename);

            string line = systemFileReader.ReadLine();
            while (line != null) {
               if (Regex.IsMatch(line, searchPattern)) {
                  references.Add(line);
               }

               line = systemFileReader.ReadLine();
            }

            return references;
         }
         finally {
            systemFileReader.Close();
         }
      }
   }
}
