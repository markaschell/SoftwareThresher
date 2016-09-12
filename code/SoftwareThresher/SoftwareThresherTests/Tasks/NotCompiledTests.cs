using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Tasks {
   [TestClass]
   public class NotCompiledTests {
      Search search;

      NotCompiled notCompiled;

      [TestInitialize]
      public void Setup() {
         search = Substitute.For<Search>();

         notCompiled = new NotCompiled(search);
      }

      [TestMethod]
      public void Execute_GetsCompiledConfigurationFiles() {
         var directory = "test it";
         notCompiled.Directory = directory;

         var fileSearchPattern = "*.cs";
         notCompiled.CompileConfigurationFileSearchPattern = fileSearchPattern;

         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string>());

         var results = notCompiled.Execute(new List<Observation>());

         search.Received().GetFiles(directory, fileSearchPattern);
      }

      [TestMethod]
      public void Execute_MulitpleCopileConfigurationsFound() {
         var textSearchPattern = "*ksdkfj*";
         notCompiled.TextSearchPattern = textSearchPattern;

         var filenanme1 = "yes";
         var filenanme2 = "no";
         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { filenanme1, filenanme2 });
         search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string>());

         var results = notCompiled.Execute(new List<Observation>());

         search.Received().GetReferencesInFile(filenanme2, textSearchPattern);
         search.Received().GetReferencesInFile(filenanme2, textSearchPattern);
      }

      [TestMethod]
      public void Execute_NoReferencesFound_ObservationFailed() {
         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { "" });
         search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string>());

         var observation = Substitute.For<Observation>();
         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(observation.Failed);
      }

      //[TestMethod]
      //public void Execute_ReferenceMatchesObservation_ObservationPasses() {
      //   search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { "" });

      //   search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { "" });

      //   var observation = Substitute.For<Observation>();
      //   var results = notCompiled.Execute(new List<Observation> { observation });

      //   Assert.IsFalse(observation.Failed);
      //}
   }
}