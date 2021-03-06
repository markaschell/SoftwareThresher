﻿using SoftwareThresher.Configurations;
using SoftwareThresher.Settings;

namespace SoftwareThresherTests.Configurations.TypeStubs {

   public interface ITestSettingWithAttributes : Setting { }

   public class TestSettingWithAttributes : ITestSettingWithAttributes {
      public string Attribute2 { get; set; }
      public string Attribute1 { get; set; }

      [Optional]
      public string AaOptionalAttr2 { get; set; }

      [Optional]
      public string AaOptionalAttr1 { get; set; }

      string PrivateAttribute { get; set; }

      public string GetOnlyAttribute { get; }

      [UsageNote("Note")]
      public string AttributeWithNote { get; set; }
   }

   public class TestSettingOneAttribute : ITestSettingWithAttributes {
      public string Attribute1 { get; set; }
   }
}
