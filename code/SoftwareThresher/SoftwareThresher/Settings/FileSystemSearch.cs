using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareThresher.Utilities;
using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Settings {
   public class FileSystemSearch : Search {
      readonly ISystemFileReader systemFileReader;

      public string BaseLocation { private get; set; }

      public FileSystemSearch() : this(new SystemFileReader()) { }

      public FileSystemSearch(ISystemFileReader systemFileReader) {
         this.systemFileReader = systemFileReader;
      }

      public List<Observation> GetObservations(string directory, string searchPattern)
      {
         var path = string.IsNullOrEmpty(BaseLocation) ? directory : BaseLocation + Path.DirectorySeparatorChar + directory;

         return Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories).ToList()
                         .ConvertAll(f => (Observation)new FileObservation(f));
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         try {
            var references = new List<string>();

            systemFileReader.Open(observation.ToString());

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
