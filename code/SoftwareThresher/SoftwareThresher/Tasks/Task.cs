using System.Collections.Generic;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public abstract class Task {
      public abstract string DefaultReportHeaderText { get; }

      [Optional]
      public string ReportHeaderText { get; set; }

      public string ReportHeader => string.IsNullOrEmpty(ReportHeaderText) ? DefaultReportHeaderText : ReportHeaderText;

      public abstract List<Observation> Execute(List<Observation> observations);
   }
}
