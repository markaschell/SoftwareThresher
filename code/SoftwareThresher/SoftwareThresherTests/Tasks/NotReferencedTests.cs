using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class NotReferencedTests {
      Search search;

      NotReferenced notReferenccedInFiles;

      [TestInitialize]
      public void Setup() {
         search = Substitute.For<Search>();

         notReferenccedInFiles = new NotReferenced(search);
      }

      //[TestMethod]
      //public void Execute_ObservationReferenced() {

      //   var stringFormat = "Find it {0} now";
      //   notReferenccedInFiles.ObservationNameRegExStringFormat = stringFormat;



      //   var observation = Substitute.For<Observation>();
      //   var results = notReferenccedInFiles.Execute(new List<Observation> { observation });



      //   Assert.IsFalse(observation.Failed);
      //}

      //[TestMethod]
      //public void Execute_ObservationNotReferenced() {
      //   search.GetFiles("", "").ReturnsForAnyArgs(new List<string>());

      //   var observation = Substitute.For<Observation>();
      //   var results = notReferenccedInFiles.Execute(new List<Observation> { observation });

      //   Assert.IsTrue(observation.Failed);
      //}
   }
}
