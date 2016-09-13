using System.Collections.Generic;
using System.Linq;
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

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_ReferenceMatchesObservation_ObservationPasses() {
         var searchPattern = "search pattern";
         notCompiled.TextSearchPattern = searchPattern;

         var projectDirectory = "Base Directory";
         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { projectDirectory + "/" });

         var filename = "filenname";
         search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { searchPattern + filename });

         var observation = Substitute.For<Observation>();
         observation.Location.Returns(projectDirectory);
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NamesDoNotMatch_ObservationFailed() {
         var searchPattern = "search pattern";
         notCompiled.TextSearchPattern = searchPattern;

         var projectDirectory = "Base Directory";
         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { projectDirectory + "/" });

         search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { searchPattern + "Filenname" });

         var observation = Substitute.For<Observation>();
         observation.Location.Returns(projectDirectory);
         observation.Name.Returns("filenname");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MultipleMatches_AllObservationsPass() {
         var searchPattern = "search pattern";
         notCompiled.TextSearchPattern = searchPattern;

         var projectDirectory = "Base Directory";
         search.GetFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { projectDirectory + "/" });

         var filename1 = "filenname1";
         var filename2 = "filenname2";
         search.GetReferencesInFile(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<string> { searchPattern + filename2, searchPattern + filename1 });

         var observation1 = Substitute.For<Observation>();
         observation1.Location.Returns(projectDirectory);
         observation1.Name.Returns(filename1);

         var observation2 = Substitute.For<Observation>();
         observation2.Location.Returns(projectDirectory);
         observation2.Name.Returns(filename2);

         var results = notCompiled.Execute(new List<Observation> { observation1, observation2 });

         Assert.IsTrue(results.TrueForAll(o => o.Failed == false));
      }
   }
}