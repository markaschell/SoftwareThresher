using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class FindFilesOnDiskTests : TaskTestBase {
      [TestMethod]
      public void FindFilesOnDisk() {
         RunTest("FindFilesOnDisk.xml");

         Assert.AreEqual(1, LastReportItem.ChangeInObservations);
         Assert.AreEqual(0, LastReportItem.FailedObservations.Count);
      }
   }
}
