using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class FindFilesOnDiskTests {
      ISystemDirectory directory;

      FindFilesOnDisk findFilesOnDisk;

      [TestInitialize]
      public void Setup() {
         directory = Substitute.For<ISystemDirectory>();

         findFilesOnDisk = new FindFilesOnDisk(directory);
      }

      [TestMethod]
      public void Execute_CreatesFileObservations() {
         var location = "location";
         var pattern = "kljasdf";
         findFilesOnDisk.Location = location;
         findFilesOnDisk.SearchPattern = pattern;

         directory.GetFiles(location, pattern).Returns(new List<string> { "one" });

         var results = findFilesOnDisk.Execute(new List<Observation>());

         Assert.AreEqual(1, results.Count);
         Assert.AreEqual(typeof(FileObservation), results.First().GetType());
      }

      [TestMethod]
      public void Execute_AddsToPassedInObservations() {
         directory.GetFiles("", "").ReturnsForAnyArgs(new List<string> { "one" });

         var results = findFilesOnDisk.Execute(new List<Observation> { new FileObservation("") });

         Assert.AreEqual(2, results.Count);
      }
   }
}
