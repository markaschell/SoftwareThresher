using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings;

namespace SoftwareThresher.Tasks {
   public class FindFilesOnDisk : Task {
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      readonly Search search;

      public FindFilesOnDisk(Search search) {
         this.search = search;
      }

      public string ReportTitle => "Found Files on Disk";

      public List<Observation> Execute(List<Observation> observations) {
         var foundItems = search.GetObservations(Directory, SearchPattern);

         observations.AddRange(foundItems);

         return observations;
      }
   }
}
