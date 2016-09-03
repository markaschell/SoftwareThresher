using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public class RegExFilter : Task, NoDetailsInReport {

      public string FilterPattern { get; set; }

      public string ReportTitle { get { return "Items Filtered"; } }

      public List<Observation> Execute(List<Observation> observations) {
         var regex = new Regex(FilterPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);

         return observations.FindAll(o => !regex.IsMatch(o.Location + Path.PathSeparator + o.Name));
      }
   }
}
