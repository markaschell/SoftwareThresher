using SoftwareThresher.Settings;

namespace SoftwareThresherTests.Configurations.TypeStubs {

   public interface ITestSettingWithAttributes : Setting { }

   public class TestSettingTwoAttributes : ITestSettingWithAttributes {
      public string Attribute2 { get; set; }
      public string Attribute1 { get; set; }

      string PrivateAttribute { get; set; }

      public string GetOnlyAttribute { get; }
   }

   public class TestSettingOneAttribute : ITestSettingWithAttributes {
      public string Attribute1 { get; set; }
   }
}
