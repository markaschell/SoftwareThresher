using System.Collections.Generic;

namespace SoftwareThresher.Settings {
   public interface Search {
      string BaseLocation { set; }

      List<string> GetFiles(string directory, string searchPattern);

      List<string> GetReferencesInFile(string filename, string searchPattern);
   }
}