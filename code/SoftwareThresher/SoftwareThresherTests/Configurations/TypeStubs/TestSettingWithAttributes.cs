using SoftwareThresher.Settings;

namespace SoftwareThresherTests.Configurations.TypeStubs {

   public interface ITestSettingWithAttributes : Setting { }

   public class TestSettingWithAttributes : ITestSettingWithAttributes {
      public string Attribute1 { get; set; }
      public string Attribute2 { get; set; }

      string PrivateAttribute { get; set; }

      public string GetOnlyAttribute { get; }
   }
}
