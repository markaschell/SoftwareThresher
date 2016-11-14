using System.IO;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Observations {
   public class OpenGrokObservation : Observation {
      const string DirectorySeperator = @"\/";

      readonly string directory;

      public OpenGrokObservation(string directory, string filename, Search search) : base(search) {
         this.directory = directory;
         Name = filename;
      }

      public override string Name { get; }

      public override string Location => directory.Replace(DirectorySeperator, Path.DirectorySeparatorChar.ToString());

      public override string SystemSpecificString {
         get {
            const string searchPathSeperator = @"/";
            var location = directory.Replace(DirectorySeperator, searchPathSeperator);

            return $"{location}{searchPathSeperator}{Name}";
         }
      }
   }
}
