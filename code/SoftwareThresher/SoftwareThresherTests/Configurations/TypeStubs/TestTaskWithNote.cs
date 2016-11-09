using System;
using System.Collections.Generic;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations.TypeStubs {
   [UsageNote("Note")]
   public class TestTaskWithNote : Task
   {
      public override string DefaultReportHeaderText => "";

      public override List<Observation> Execute(List<Observation> observations)
      {
         throw new NotImplementedException();
      }
   }
}
