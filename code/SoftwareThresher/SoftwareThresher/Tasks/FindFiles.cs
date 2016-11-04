using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   public class FindFiles : Task {
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      readonly Search search;

      public FindFiles(Search search) {
         this.search = search;
      }

      public string ReportTitle => "Files Found";

      public List<Observation> Execute(List<Observation> observations) {
         var foundItems = search.GetObservations(Directory, SearchPattern);

         observations.AddRange(foundItems);

         return observations;
      }
   }
}
