using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Observations;

namespace SoftwareThresherTests.Observations {
   [TestClass]
   public class FileObservationTests {
      [TestMethod]
      public void Name_JustName() {
         const string name = "name";

         var observation = new FileObservation(name);

         Assert.AreEqual(name, observation.Name);
      }

      [TestMethod]
      public void Name_NameWithExtension() {
         const string name = "name.txt";

         var observation = new FileObservation(name);

         Assert.AreEqual(name, observation.Name);
      }

      [TestMethod]
      public void Name_NameWithPath() {
         const string name = "name";

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
         const string name = "name";

         var observation = new FileObservation(name);

         Assert.AreEqual(string.Empty, observation.Location);
      }

      [TestMethod]
      public void Location_PathWithName() {
         const string path = @"C:\Directory";

         var observation = new FileObservation(path + @"\name");

         Assert.AreEqual(path, observation.Location);
      }

      [TestMethod]
      public void Location_JustPath() {
         const string path = @"C:\Directory";

         var observation = new FileObservation(path + @"\");

         Assert.AreEqual(path, observation.Location);
      }

      [TestMethod]
      public void ToString_CombinesLocationAndFilenanme() {
         const string filename = @"C:\Directory\name.txt";

         var observation = new FileObservation(filename);

         Assert.AreEqual(filename, observation.ToString());
      }

      [TestMethod]
      public void ToString_JustPath() {
         const string path = @"C:\Directory\";

         var observation = new FileObservation(path);

         Assert.AreEqual(path, observation.ToString());
      }

      [TestMethod]
      public void ToString_JustFile() {
         const string file = @"test.txt";

         var observation = new FileObservation(file);

         Assert.AreEqual(file, observation.ToString());
      }

   }

}
