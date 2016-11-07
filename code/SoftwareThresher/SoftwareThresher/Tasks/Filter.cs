using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public class Filter : Task {

      [Optional, UsageNote("Format is RegEx")]
      public string SearchPattern { get; set; }

      // TODO - will this work as not a string - Add tests in configruation for this?
      [Optional, UsageNote("Postive Number")]
      public double EditedInDays { get; set; }

      public override string DefaultReportHeaderText => "Items Filtered";

      public override List<Observation> Execute(List<Observation> observations) {
         var passedObservations = FilterBySearchPattern(observations);

         return FilterByEditAge(passedObservations);
      }

      List<Observation> FilterBySearchPattern(List<Observation> observations) {
         if (string.IsNullOrEmpty(SearchPattern)) return observations;

         var regex = new Regex(SearchPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);
         observations.RemoveAll(o => regex.IsMatch(o.ToString()));

         return observations;
      }

      List<Observation> FilterByEditAge(List<Observation> observations) {
         if (EditedInDays > 0) {
            observations.RemoveAll(o => o.LastEdit.DaysOld <= EditedInDays);
         }

         return observations;
      }
   }
}
