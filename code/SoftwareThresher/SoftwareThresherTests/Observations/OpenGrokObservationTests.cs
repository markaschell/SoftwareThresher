using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Observations;

namespace SoftwareThresherTests.Observations {
   [TestClass]
   public class OpenGrokObservationTests {
      [TestMethod]
      public void Location_ReturnsStandardSystemPath() {
         var observation = new OpenGrokObservation(@"\/source\/test", "", null);

         Assert.AreEqual(@"\source\test", observation.Location);
      }

      [TestMethod]
      public void SystemSpecificString_ReturnsPathWithForwardSlashes() {
         var observation = new OpenGrokObservation(@"\/source\/test", "file.cs", null);

         Assert.AreEqual(@"/source/test/file.cs", observation.SystemSpecificString);
      }

      [TestMethod]
      public void SystemSpecificString_Fileonly() {
         var observation = new OpenGrokObservation("", "file.cs", null);

         Assert.AreEqual(@"/file.cs", observation.SystemSpecificString);
      }
   }
}
