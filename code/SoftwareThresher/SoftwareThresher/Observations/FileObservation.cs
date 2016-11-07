using System;
using System.IO;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Observations {
   public class FileObservation : Observation {
      readonly string filename;

      public FileObservation(string filename) {
         this.filename = filename;
      }

      public override string Name => Path.GetFileName(filename);

      public override string Location => Path.GetDirectoryName(filename);

      public override Date LastEdit { get { throw new NotImplementedException(); } }

      public override string SystemSpecificString => filename;
   }
}
