using System;
using System.Collections.Generic;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations.TypeStubs {
   public class TestTask : Task
   {
      public string ReportTitle => "";

      public string Attribute2 { get; set; }

      public string Attribute1 { get; set; }

      [UsageNote("Note")]
      public string AttributeWithNote { get; set; }

      string PrivateAttribute { get; set; }

      public List<Observation> Execute(List<Observation> observations)
      {
         throw new NotImplementedException();
      }
   }
}
