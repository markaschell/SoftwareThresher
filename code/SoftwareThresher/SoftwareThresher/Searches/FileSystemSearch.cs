using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using SoftwareThresher.Utilities;
using System.Text.RegularExpressions;

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
         try {
            var references = new List<string>();

            systemFileReader.Open(filename);

            string line = systemFileReader.ReadLine();
            while (line != null) {
               if (Regex.IsMatch(line, searchPattern)) {
                  references.Add(line);
               }

               line = systemFileReader.ReadLine();
            };

            return references;
         }
         finally {
            systemFileReader.Close();
         }
      }
   }
}
