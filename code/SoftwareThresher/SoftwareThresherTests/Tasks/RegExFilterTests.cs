using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class RegExFilterTests {

      RegExFilter regExFilter;

      [TestInitialize]
      public void Setup() {
         regExFilter = new RegExFilter();
      }

      [TestMethod]
      public void Execute_FiltersItem() {
         regExFilter.FilterPattern = "i";

         var result = regExFilter.Execute(new List<Observation> { new FileObservation("this is it") });

         Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public void Execute_DoesNotFilterItem() {
         regExFilter.FilterPattern = "a";

         var result = regExFilter.Execute(new List<Observation> { new FileObservation("this is it") });

         Assert.AreEqual(1, result.Count);
      }
   }
}
