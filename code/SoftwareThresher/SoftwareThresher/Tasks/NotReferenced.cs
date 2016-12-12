using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   // TODO try this out.  
   public class NotReferenced : Task {

      [Optional, UsageNote("Format is RegEx")]
      public string ReferenceFilesToIgnore { get; set; }

      public override string DefaultReportHeaderText => "Not Referenced";

      readonly Search search;

      public NotReferenced(Search search) {
         this.search = search;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         foreach (var observation in observations) {
            var observationName = observation.FilenameWithoutExtension;

            var references = search.GetObservations(string.Empty, observationName);

            if (!string.IsNullOrEmpty(ReferenceFilesToIgnore))
            {
               references.RemoveAll(r => Regex.IsMatch(r.Name, ReferenceFilesToIgnore));
            }

            if (references.Count(r => r.FilenameWithoutExtension != observationName) == 0) {
               observation.Failed = true;
            }
         }

         return observations;
      }
   }
}
