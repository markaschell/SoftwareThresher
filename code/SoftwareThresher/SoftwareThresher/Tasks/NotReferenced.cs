using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   public class NotReferenced : Task {
      // TODO - Can we build into the base to just return the class name?
      public override string DefaultReportHeaderText => "Not Referenced";

      readonly Search search;

      public NotReferenced(Search search) {
         this.search = search;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         foreach (var observation in observations) {
            var observationName = observation.FilenameWithoutExtension;
            var references = search.GetObservations(string.Empty, observationName);

            if (references.Any(r => r.FilenameWithoutExtension != observationName)) {
               observation.Failed = true;
            }
         }

         return observations;
      }
   }
}
