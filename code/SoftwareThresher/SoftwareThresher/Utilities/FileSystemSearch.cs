using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoftwareThresher.Utilities {
   public class FileSystemSearch : Search {
      public List<string> GetFiles(string location, string searchPattern) {
         return Directory.EnumerateFiles(location, searchPattern, SearchOption.AllDirectories).ToList();
      }
      public List<Reference> FindInFile(string location, string fileSearchPattern, string textSearchPattern) {
         throw new NotImplementedException();
      }
   }
}
