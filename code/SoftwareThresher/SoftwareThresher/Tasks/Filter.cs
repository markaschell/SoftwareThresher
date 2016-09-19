using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public class Filter : Task {

      // TODO - make nullable when the next parameter is added
      // NOTE - RegEx format
      public string LocationSearchPattern { get; set; }

      public string ReportTitle { get { return "Items Filtered"; } }

      public List<Observation> Execute(List<Observation> observations) {
         var regex = new Regex(LocationSearchPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);

         return observations.FindAll(o => !regex.IsMatch(o.Location));
      }
   }
}
