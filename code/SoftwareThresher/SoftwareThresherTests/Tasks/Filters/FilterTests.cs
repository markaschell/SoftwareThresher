using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Tasks.Filters;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Tasks.Filters {
   [TestClass]
   public class FilterTests
   {
      Observation observation;

      Filter filter;

      [TestInitialize]
      public void Setup()
      {
         observation = Substitute.For<Observation>((Search)null);

         filter = new Filter();
      }

      [TestMethod]
      public void Execute_AllMatch_Filters() {
         filter.SearchPattern = "a";

         const int daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit;

         observation.ToString().Returns(@"C:\akdkk\this is it");
         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-daysSinceEdit)));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(0, results.Count);
      }

      [TestMethod]
      public void Execute_OnlySearchMatches_Filters() {
         filter.SearchPattern = "a";

         observation.ToString().Returns(@"C:\akdkk\this is it");

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(0, results.Count);
      }

      [TestMethod]
      public void Execute_OnlyEditInDaysMatches_Filters() {
         const int daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit;

         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-daysSinceEdit)));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(0, results.Count);
      }

      [TestMethod]
      public void Execute_DoesNotMatchSearch_DoesNotFilter() {
         filter.SearchPattern = "z";

         const int daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit;

         observation.ToString().Returns(@"C:\akdkk\this is it");
         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-(daysSinceEdit))));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(1, results.Count);
         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_DoesNotMatchEditInDays_DoesNotFilter() {
         filter.SearchPattern = "a";

         const int daysSinceEdit = 5;
         filter.EditedInDays = daysSinceEdit;

         observation.ToString().Returns(@"C:\akdkk\this is it");
         observation.LastEdit.Returns(new Date(DateTime.Today.AddDays(-(daysSinceEdit + 1))));

         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(1, results.Count);
         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NoFilter_DoesNotFilter() {
         var results = filter.Execute(new List<Observation> { observation });

         Assert.AreEqual(1, results.Count);
         Assert.IsFalse(results.First().Failed);
      }
   }
}
