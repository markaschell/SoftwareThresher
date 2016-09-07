using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public class Filter : Task, NoDetailsInReport {

      // TODO - make nullable when the next parameter is added
      public string LocationRegExPattern { get; set; }

      public string ReportTitle { get { return "Items Filtered"; } }

      public List<Observation> Execute(List<Observation> observations) {
         var regex = new Regex(LocationRegExPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);

         return observations.FindAll(o => !regex.IsMatch(o.Location));
      }
   }
}
