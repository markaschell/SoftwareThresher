using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class DoesNothingTests : TaskTestBase {
      [TestMethod]
      public void DoesNothing_NoResult() {
         RunTest("NoSettingsOrTasks.xml");

         Assert.IsNull(LastReportItem);
      }

      [TestMethod]
      public void NoSections_NoResult() {
         RunTest("NoSections.xml");

         Assert.IsNull(LastReportItem);
      }
   }
}
