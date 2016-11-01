using System.IO;

namespace SoftwareThresher.Observations {
   public abstract class Observation {
      public virtual bool Failed { get; set; }

      public abstract string Name { get; }

      public abstract string Location { get; }

      public abstract string SystemSpecificString { get; }

      public override string ToString() {
         return $"{Location}{Path.PathSeparator}{Name}";
      }
   }
}
