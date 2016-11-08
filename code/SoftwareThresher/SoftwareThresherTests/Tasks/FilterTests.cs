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
   public class FilterTests
   {
      Observation observation;

      Filter filter;

      [TestInitialize]
      public void Setup()
      {
         observation = Substitute.For<Observation>();

         filter = new Filter();
      }

      [TestMethod]
      public void Execute_FiltersLocationRegExPattern() {
         filter.SearchPattern = "a";

         observation.ToString().Returns(@"C:\akdkk\this is it");

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(0, results.Count);
      }

      [TestMethod]
      public void Execute_DoesNotFilter() {
         filter.SearchPattern = "z";

         const double daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit;

         observation.ToString().Returns(@"C:\akdkk\this is it");
         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-(daysSinceEdit + 1))));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(1, results.Count);
         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_FiltersLastEdit() {
         const double daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit + .1;

         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-daysSinceEdit)));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(0, results.Count);
      }
   }
}
