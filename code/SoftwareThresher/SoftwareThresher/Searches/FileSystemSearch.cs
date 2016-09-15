using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Searches {
   public class FileSystemSearch : Search {

      ISystemFileReader systemFileReader;

      public FileSystemSearch() : this(new SystemFileReader()) { }

      public FileSystemSearch(ISystemFileReader systemFileReader) {
         this.systemFileReader = systemFileReader;
      }

      public List<string> GetFiles(string directory, string searchPattern) {
         return Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
      }

      public List<string> GetReferencesInFile(string filename, string searchPattern) {
         throw new NotImplementedException();
      }
   }
}
