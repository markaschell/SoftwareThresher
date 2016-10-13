﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Settings;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class FindFilesOnDiskTests {
      Search systemDirectory;

      FindFilesOnDisk findFilesOnDisk;

      [TestInitialize]
      public void Setup() {
         systemDirectory = Substitute.For<Search>();

         findFilesOnDisk = new FindFilesOnDisk(systemDirectory);
      }

      [TestMethod]
      public void Execute_CreatesFileObservations() {
         var directory = "location";
         var pattern = "kljasdf";
         findFilesOnDisk.Directory = directory;
         findFilesOnDisk.SearchPattern = pattern;

         systemDirectory.GetObservations(directory, pattern).Returns(new List<Observation> { new FileObservation("one") });

         var results = findFilesOnDisk.Execute(new List<Observation>());

         Assert.AreEqual(1, results.Count);
         Assert.AreEqual(typeof(FileObservation), results.First().GetType());
         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_AddsToPassedInObservations() {
         systemDirectory.GetObservations("", "").ReturnsForAnyArgs(new List<Observation> { new FileObservation("one") });

         var results = findFilesOnDisk.Execute(new List<Observation> { new FileObservation("") });

         Assert.AreEqual(2, results.Count);
      }
   }
}
