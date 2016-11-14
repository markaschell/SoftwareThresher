using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class FindFilesTests {
      Search systemDirectory;

      FindFiles findFiles;

      [TestInitialize]
      public void Setup() {
         systemDirectory = Substitute.For<Search>();

         findFiles = new FindFiles(systemDirectory);
      }

      [TestMethod]
      public void Execute_CreatesFileObservations() {
         var directory = "location";
         var pattern = "kljasdf";
         findFiles.Directory = directory;
         findFiles.SearchPattern = pattern;

         systemDirectory.GetObservations(directory, pattern).Returns(new List<Observation> { new FileObservation("one", null) });

         var results = findFiles.Execute(new List<Observation>());

         Assert.AreEqual(1, results.Count);
         Assert.AreEqual(typeof(FileObservation), results.First().GetType());
         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_AddsToPassedInObservations() {
         systemDirectory.GetObservations("", "").ReturnsForAnyArgs(new List<Observation> { new FileObservation("one", null) });

         var results = findFiles.Execute(new List<Observation> { new FileObservation("", null) });

         Assert.AreEqual(2, results.Count);
      }
   }
}
