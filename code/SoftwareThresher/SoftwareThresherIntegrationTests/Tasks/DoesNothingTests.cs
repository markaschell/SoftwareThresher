using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class DoesNothingTests : TaskTestBase {
      [TestMethod]
      public void DoesNothing_NoResult() {
         RunTest("DoesNothing.xml");

         Assert.IsNull(LastReportItem);
      }
   }
}
