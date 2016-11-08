using System.IO;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Observations {
   public class OpenGrokObservation : Observation {
      const string DirectorySeperator = @"\/";

      readonly string directory;
      readonly Search search;

      public OpenGrokObservation(string directory, string filename, Search search) {
         this.directory = directory;
         Name = filename;

         this.search = search;
      }

      public override string Name { get; }

      public override string Location => directory.Replace(DirectorySeperator, Path.DirectorySeparatorChar.ToString());

      // TODO - push more logic into this function?
      // TODO - get rid of duplication with FileObservation?
      public override Date LastEdit => search.GetLastEditDate(this);

      public override string SystemSpecificString {
         get {
            const string searchPathSeperator = @"/";
            var location = directory.Replace(DirectorySeperator, searchPathSeperator);

            return $"{location}{searchPathSeperator}{Name}";
         }
      }
   }
}
