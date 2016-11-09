using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Configurations;
using SoftwareThresher.Settings;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Tasks;
using SoftwareThresher.Tasks.Filters;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class AssemblyObjectFinderTests {
      AssemblyObjectFinder assemblyObjectFinder;

      [TestInitialize]
      public void Setup() {
         assemblyObjectFinder = new AssemblyObjectFinder();
      }

      [TestMethod]
      public void TaskTypes_FindsTasks() {
         var types = assemblyObjectFinder.TaskTypes.ToList();

         Assert.IsTrue(types.Any());
         Assert.IsTrue(types.Contains(typeof(Filter)));
      }

      [TestMethod]
      public void TaskTypes_DoesNotIncludeOtherClasses() {
         var types = assemblyObjectFinder.TaskTypes.ToList();

         Assert.IsFalse(types.Contains(typeof(Configuration)));
      }

      [TestMethod]
      public void TaskTypes_DoesNotIncludeTask()
      {
         var types = assemblyObjectFinder.TaskTypes;

         Assert.IsFalse(types.Contains(typeof(Task)));
      }

      [TestMethod]
      public void SettingTypes_FindsSettings() {
         var types = assemblyObjectFinder.SettingTypes.ToList();

         Assert.IsTrue(types.Any());
         Assert.IsTrue(types.Contains(typeof(FileSystemSearch)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeOtherClasses() {
         var types = assemblyObjectFinder.SettingTypes.ToList();

         Assert.IsFalse(types.Contains(typeof(Configuration)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeSetting() {
         var types = assemblyObjectFinder.SettingTypes;

         Assert.IsFalse(types.Contains(typeof(Setting)));
      }

      [TestMethod]
      public void SettingTypes_DoesNotIncludeInterfaces() {
         var types = assemblyObjectFinder.SettingTypes;

         Assert.IsFalse(types.Contains(typeof(Search)));
      }

      [TestMethod]
      public void SettingInterfaces_FindsSettingInterfaces() {
         var types = assemblyObjectFinder.SettingInterfaces.ToList();

         Assert.IsTrue(types.Any());
         Assert.IsTrue(types.Contains(typeof(Search)));
      }

      [TestMethod]
      public void SettingInterfaces_DoesNotIncludeOtherInterfaces() {
         var types = assemblyObjectFinder.SettingInterfaces.ToList();

         Assert.IsFalse(types.Contains(typeof(IConfiguration)));
      }

      [TestMethod]
      public void SettingInterfaces_DoesNotIncludeSetting() {
         var types = assemblyObjectFinder.SettingInterfaces;

         Assert.IsFalse(types.Contains(typeof(Setting)));
      }

      [TestMethod]
      public void SettingInterfaces_DoesNotIncludeClasses() {
         var types = assemblyObjectFinder.SettingInterfaces;

         Assert.IsFalse(types.Contains(typeof(FileSystemSearch)));
      }

   }
}
