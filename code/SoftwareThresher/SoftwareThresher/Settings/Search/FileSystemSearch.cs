using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Settings.Search {
   public class FileSystemSearch : Search {
      readonly ISystemFileReader systemFileReader;

      [Optional]
      public string BaseLocation { private get; set; }

      public FileSystemSearch() : this(new SystemFileReader()) { }

      public FileSystemSearch(ISystemFileReader systemFileReader) {
         this.systemFileReader = systemFileReader;
      }

      public List<Observation> GetObservations(string location, string searchPattern)
      {
         var path = string.IsNullOrEmpty(BaseLocation) ? location : BaseLocation + Path.DirectorySeparatorChar + location;

         return Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories).ToList()
                         .ConvertAll(f => (Observation)new FileObservation(f, this));
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         try {
            var references = new List<string>();

            systemFileReader.Open(observation.SystemSpecificString);

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

      // TODO - immplement?  Note or a Task on the web
      public Date GetLastEditDate(Observation observation)
      {
         return Date.NullDate;
      }
   }
}
