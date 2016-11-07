using System;
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
         var passedObservations = observations;
         if (!string.IsNullOrEmpty(SearchPattern))
         {
            var regex = new Regex(SearchPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);
            passedObservations.RemoveAll(o => regex.IsMatch(o.ToString()));
         }

         if (EditedInDays > 0)
         {
            passedObservations.RemoveAll(o => o.LastEdit.DaysOld <= EditedInDays);
         }

         return passedObservations;
      }
   }
}
