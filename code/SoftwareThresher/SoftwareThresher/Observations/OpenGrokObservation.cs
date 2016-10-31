using System;
using System.IO;

namespace SoftwareThresher.Observations {
   public class OpenGrokObservation : Observation {
      readonly string directory;

      public OpenGrokObservation(string directory, string filename)
      {
         this.directory = directory;
         this.Name = filename;
      }

      public override string Location => directory.Replace(@"\\\", string.Empty);

      public override string Name { get; }

      public override string ToString() {
         return Location + '/' + Name;
      }
   }
}
