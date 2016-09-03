using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Observations;

namespace SoftwareThresherTests.Observations {
   [TestClass]
   public class FileObservationTests {
      [TestMethod]
      public void Name_JustName() {
         var name = "name";

         var observation = new FileObservation(name);

         Assert.AreEqual(name, observation.Name);
      }

      [TestMethod]
      public void Name_NameWithExtension() {
         var name = "name";

         var observation = new FileObservation(name);

         Assert.AreEqual(name, observation.Name);
      }

      [TestMethod]
      public void Name_NameWithPath() {
         var name = "name";

         var observation = new FileObservation(@"C:\Directory\" + name);

         Assert.AreEqual(name, observation.Name);
      }

      [TestMethod]
      public void Name_JustPath() {
         var observation = new FileObservation(@"C:\Directory\");

         Assert.AreEqual(string.Empty, observation.Name);
      }

      [TestMethod]
      public void Location_JustName() {
         var name = "name";

         var observation = new FileObservation(name);

         Assert.AreEqual(string.Empty, observation.Location);
      }

      [TestMethod]
      public void Location_PathWithName() {
         var path = @"C:\Directory";

         var observation = new FileObservation(path + @"\name");

         Assert.AreEqual(path, observation.Location);
      }

      [TestMethod]
      public void Location_JustPath() {
         var path = @"C:\Directory";

         var observation = new FileObservation(path + @"\");

         Assert.AreEqual(path, observation.Location);
      }

   }

}
