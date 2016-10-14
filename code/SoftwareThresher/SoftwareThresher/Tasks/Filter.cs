using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public class Filter : Task {

      [UsageNote("Format is RegEx")]
      public string LocationSearchPattern { get; set; }

      public string ReportTitle => "Items Filtered";

      public List<Observation> Execute(List<Observation> observations) {
         var regex = new Regex(LocationSearchPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);

         return observations.FindAll(o => !regex.IsMatch(o.Location));
      }
   }
}
