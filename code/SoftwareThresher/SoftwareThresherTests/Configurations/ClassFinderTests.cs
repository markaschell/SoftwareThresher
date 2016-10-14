using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Configurations;
using SoftwareThresher.Settings;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class ClassFinderTests {
      ClassFinder classFinder;

      [TestInitialize]
      public void Setup() {
         classFinder = new ClassFinder();
      }

      [TestMethod]
      public void TaskTypes_FindsTasks() {
         var types = classFinder.TaskTypes.ToList();

         Assert.IsTrue(types.Any());
         Assert.IsTrue(types.Contains(typeof(Filter)));
      }

      [TestMethod]
      public void TaskTypes_DoesNotIncludeOtherClasses() {
         var types = classFinder.TaskTypes.ToList();

         Assert.IsFalse(types.Contains(typeof(Configuration)));
      }

      [TestMethod]
      public void TaskTypes_DoesNotIncludeTask()
      {
         var types = classFinder.TaskTypes;

         Assert.IsFalse(types.Contains(typeof(Task)));
      }

      [TestMethod]
      public void SettingTypes_FindsSettings() {
         var types = classFinder.SettingTypes.ToList();

         Assert.IsTrue(types.Any());
         Assert.IsTrue(types.Contains(typeof(FileSystemSearch)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeOtherClasses() {
         var types = classFinder.SettingTypes.ToList();

         Assert.IsFalse(types.Contains(typeof(Configuration)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeSetting() {
         var types = classFinder.SettingTypes;

         Assert.IsFalse(types.Contains(typeof(Setting)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeInterfaces() {
         var types = classFinder.SettingTypes;

         Assert.IsFalse(types.Contains(typeof(Search)));
      }
   }
}
