using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class NotReferencedTests {

      Search search;

      NotReferenced notReferenced;

      [TestInitialize]
      public void Setup() {
         search = Substitute.For<Search>();

         notReferenced = new NotReferenced(search);
      }

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void Execute_GetReferences()
      {
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation>());

         const string name = "name";
         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name);

         notReferenced.Execute(new List<Observation> { suppliedObservation });

         search.Received().GetObservations(string.Empty, name);
      }

      [TestMethod]
      public void Execute_GetReferences_RemovesFileExtension() {
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation>());

         const string name = "name";
         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name + ".txt");

         notReferenced.Execute(new List<Observation> { suppliedObservation });

         search.Received().GetObservations(string.Empty, name);
      }

      [TestMethod]
      public void Execute_DifferentNameReferenceFound_Fails() {
         const string name = "name";

         var foundObservation1 = ObservationStub;
         foundObservation1.Name.Returns(name);

         var foundObservation2 = ObservationStub;
         foundObservation2.Name.Returns("otherName");

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { foundObservation1, foundObservation2 });

         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name);

         var results = notReferenced.Execute(new List<Observation> { suppliedObservation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NoReferencesFound_Passes() {
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation>());

         var results = notReferenced.Execute(new List<Observation> { ObservationStub });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingNameReferenceFound_Passes()
      {
         const string name = "name";

         var foundObservation = ObservationStub;
         foundObservation.Name.Returns(name);

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { foundObservation });

         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name);

         var results = notReferenced.Execute(new List<Observation> { suppliedObservation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingNameReferenceFoundWithExtenstion_Passes() {
         const string name = "name";

         var foundObservation = ObservationStub;
         foundObservation.Name.Returns(name + ".txt");

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { foundObservation });

         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name);

         var results = notReferenced.Execute(new List<Observation> { suppliedObservation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingNameWithExtensionReferenceFound_Passes() {
         const string name = "name";

         var foundObservation = ObservationStub;
         foundObservation.Name.Returns(name);

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { foundObservation });

         var suppliedObservation = ObservationStub;
         suppliedObservation.Name.Returns(name + ".txt");

         var results = notReferenced.Execute(new List<Observation> { suppliedObservation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_Multiple() {
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { ObservationStub });

         var name = "name";
         var failedObservation = ObservationStub;
         failedObservation.Name.Returns(name);

         var results = notReferenced.Execute(new List<Observation> { failedObservation, ObservationStub, failedObservation });

         search.Received().GetObservations(Arg.Any<string>(), name);
         search.Received().GetObservations(Arg.Any<string>(), string.Empty);

         Assert.AreEqual(2, results.FindAll(o => o.Failed).Count);
      }
   }
}
