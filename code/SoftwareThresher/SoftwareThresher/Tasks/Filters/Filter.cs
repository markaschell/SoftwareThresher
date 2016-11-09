using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks.Filters {
   [UsageNote("All attributes must match to filter")]
   public class Filter : Task {

      [Optional, UsageNote("Format is RegEx")]
      public string SearchPattern { get; set; }

      // TODO - will this work as not a string - Add tests in configruation for this?  At minimum we should have a different error if the value cannot be set becasue of type
      // TODO - test this out
      [Optional, UsageNote("Postive Number")]
      public double EditedInDays { get; set; }

      public override string DefaultReportHeaderText => "Items Filtered";

      public override List<Observation> Execute(List<Observation> observations) {
         var filters = new List<FilterParameter> {
            new SearchPatternFilterParameter(SearchPattern),
            new EditAgeFilterParameter(EditedInDays)
         };

         if (filters.All(f => !f.IsDefined)) {
            return observations;
         }

         observations.RemoveAll(o => filters.All(f => !f.IsDefined || f.ShouldFilter(o)));

         return observations;
      }
   }
}
