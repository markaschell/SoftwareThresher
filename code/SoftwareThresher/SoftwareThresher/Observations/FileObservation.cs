using System.IO;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Observations {
   public class FileObservation : Observation {
      readonly string filename;

      public FileObservation(string filename, Search search) : base(search) {
         this.filename = filename;
      }

      public override string Name => Path.GetFileName(filename);

      public override string Location => Path.GetDirectoryName(filename);

      public override string SystemSpecificString => filename;
   }
}
