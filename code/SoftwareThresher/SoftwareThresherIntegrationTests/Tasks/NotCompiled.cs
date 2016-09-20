using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   [TestClass]
   public class NotCompiledTests : TaskTestBase {
      [TestMethod]
      public void NotCompiled() {
         RunTest("NotCompiled.xml");

         Assert.AreEqual(-1, LastReportItem.ChangeInObservations);
         Assert.AreEqual(1, LastReportItem.FailedObservations.Count);
         Assert.AreEqual("ClassNotIncludedInTheProject.cs", LastReportItem.FailedObservations.First().Name);
      }
   }
}
