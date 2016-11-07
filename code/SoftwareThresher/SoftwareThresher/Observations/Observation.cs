using System;
using System.IO;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Observations {
   public abstract class Observation {
      public virtual bool Failed { get; set; }

      public abstract string Name { get; }

      public abstract string Location { get; }

      public abstract Date LastEdit { get; }

      public abstract string SystemSpecificString { get; }

      public override string ToString() {
         return $"{Location}{Path.PathSeparator}{Name}";
      }
   }
}
