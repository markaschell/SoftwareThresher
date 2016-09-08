using System.Collections.Generic;

namespace SoftwareThresher.Utilities {
   public interface Search {
      // TODO - should this return an observation
      List<string> GetFiles(string location, string searchPattern);

      List<Reference> FindInFile(string location, string fileSearchPattern, string textSearchPattern);
   }
}
