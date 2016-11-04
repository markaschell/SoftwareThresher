using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations.TypeStubs {
   public class TestTaskWithOneSetting : Task {
      public override string DefaultReportHeaderText => "";

      readonly ITestSettingWithAttributes testSettingWithAttributes;

      public TestTaskWithOneSetting(ITestSettingWithAttributes testSettingWithAttributes)
      {
         this.testSettingWithAttributes = testSettingWithAttributes;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         throw new NotImplementedException();
      }

      public bool SettingsSet => Attribute != null;

      public TestSettingTwoAttributes Attribute => (TestSettingTwoAttributes)testSettingWithAttributes;
   }
}
