using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class FindFilesTests : TaskTestBase {
      [TestMethod]
      public void FindFiles() {
         RunTest("FindFiles.xml");

         Assert.AreEqual(1, LastReportItem.ChangeInObservations);
         Assert.AreEqual(0, LastReportItem.FailedObservations.Count);
      }
   }
}
