using System.IO;

namespace SoftwareThresher.Observations {
   public class FileObservation : Observation {
      readonly string filename;

      public FileObservation(string filename) {
         this.filename = filename;
      }

      public override string Location => Path.GetDirectoryName(filename);

      public override string Name => Path.GetFileName(filename);

      public override string ToString() {
         return Location + Path.DirectorySeparatorChar + Name;
      }
   }
}
