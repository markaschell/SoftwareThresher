using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class FilterTests : TaskTestBase {
      [TestMethod]
      public void Filter() {
         RunTest("Filter.xml");

         Assert.AreEqual(-1, LastReportItem.ChangeInObservations);
         Assert.AreEqual(0, LastReportItem.FailedObservations.Count);
      }
   }
}
