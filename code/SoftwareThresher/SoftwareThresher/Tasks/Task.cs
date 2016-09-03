using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
   public interface Task {
      string ReportTitle { get; }

      List<Observation> Execute(List<Observation> observations);
   }

   public interface NoDetailsInReport : Task {
   }
}
