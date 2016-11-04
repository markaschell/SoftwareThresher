using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations.TypeStubs {
   public class TestTaskWithTwoSettings : Task {
      readonly ITestSettingWithAttributes testSettingWithAttributes;
      readonly ITestSettingNoAttributes testSettingNoAttributes;

      public override string DefaultReportHeaderText => "";

      public TestTaskWithTwoSettings(ITestSettingWithAttributes testSettingWithAttributes, ITestSettingNoAttributes testSettingNoAttributes)
      {
         this.testSettingWithAttributes = testSettingWithAttributes;
         this.testSettingNoAttributes = testSettingNoAttributes;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         throw new NotImplementedException();
      }

      public bool SettingsSet => testSettingWithAttributes != null && testSettingNoAttributes != null;
   }
}
