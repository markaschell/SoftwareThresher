using System.Collections.Generic;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   public class FindFiles : Task {
      [Optional]
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      readonly Search search;

      public FindFiles(Search search) {
         this.search = search;
      }

      public override string DefaultReportHeaderText => "Files Found";

      public override List<Observation> Execute(List<Observation> observations) {
         var foundItems = search.GetObservations(Directory, SearchPattern);

         observations.AddRange(foundItems);

         return observations;
      }
   }
}
