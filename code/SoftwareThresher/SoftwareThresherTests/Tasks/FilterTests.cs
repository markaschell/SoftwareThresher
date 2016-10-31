using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class FilterTests {

      Filter filter;

      [TestInitialize]
      public void Setup() {
         filter = new Filter();
      }

      [TestMethod]
      public void Execute_FiltersLocationRegExPattern() {
         filter.SearchPattern = "a";

         var result = filter.Execute(new List<Observation> { new FileObservation(@"C:\akdkk\this is it") });

         Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public void Execute_DoesNotFilterLocationRegExPattern() {
         filter.SearchPattern = "z";

         var results = filter.Execute(new List<Observation> { new FileObservation(@"C:\akdkk\this is it") });

         Assert.AreEqual(1, results.Count);
         Assert.IsFalse(results.First().Failed);
      }
   }
}
