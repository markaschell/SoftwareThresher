﻿using System.Collections.Generic;

namespace SoftwareThresher.Searches {
   public interface Search {
      List<string> GetFiles(string directory, string searchPattern);

      List<string> GetReferencesInFile(string filename, string searchPattern);
   }
}