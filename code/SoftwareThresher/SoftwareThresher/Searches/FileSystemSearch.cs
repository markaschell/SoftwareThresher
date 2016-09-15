using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace SoftwareThresher.Searches {
   public class FileSystemSearch : Search {
      public List<string> GetFiles(string directory, string searchPattern) {
         return Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
      }

      public List<string> GetReferencesInFile(string filename, string searchPattern) {
         throw new NotImplementedException();
      }
   }
}
