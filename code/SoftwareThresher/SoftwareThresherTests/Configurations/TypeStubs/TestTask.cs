using System;
using System.Collections.Generic;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations.TypeStubs {
   public class TestTask : Task
   {
      public override string DefaultReportHeaderText => "";

      public string Attribute2 { get; set; }

      public string Attribute1 { get; set; }

      [Optional]
      public string AaOptionalAttr2 { get; set; }

      [Optional]
      public string AaOptionalAttr1 { get; set; }

      public string GetOnlyAttribute { get; }

      [UsageNote("Note")]
      public string AttributeWithNote { get; set; }

      public int IntAttribute { get; set; }

      string PrivateAttribute { get; set; }

      public override List<Observation> Execute(List<Observation> observations)
      {
         throw new NotImplementedException();
      }
   }
}
