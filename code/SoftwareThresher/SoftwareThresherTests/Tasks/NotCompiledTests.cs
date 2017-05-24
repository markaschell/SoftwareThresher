using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Tasks;
using SoftwareThresher.Settings.Search;

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

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void Execute_GetsCompiledConfigurationFiles() {
         const string directory = "test it";
         notCompiled.Directory = directory;

         const string fileSearchPattern = "*.cs";
         notCompiled.CompileConfigurationFileSearchPattern = fileSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation>());

         notCompiled.Execute(new List<Observation>());

         search.Received().GetObservations(directory, fileSearchPattern);
      }

      [TestMethod]
      public void Execute_MulitpleCompileConfigurationsFound() {
         const string startTextSearchPattern = "*ksdkfj*";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;

         var observation1 = new FileObservation("one", null);
         var observation2 = new FileObservation("two", null);
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { observation1, observation2 });
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string>());

         notCompiled.Execute(new List<Observation>());

         search.Received().GetReferenceLine(observation1, startTextSearchPattern);
         search.Received().GetReferenceLine(observation2, startTextSearchPattern);
      }

      [TestMethod]
      public void Execute_NoReferencesFound_Fails() {
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string>());

         var observation = ObservationStub;
         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NamesDoNotMatch_Fails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + "filnname" + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns("filenname");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MultipleMatches_AllOPass() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         var projectDirectory = "base directory";
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation(projectDirectory + "/", null) });

         const string filename1 = "filenname1";
         const string filename2 = "filenname2";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename2 + endTextSearchPattern,
                                                                                                       startTextSearchPattern + filename1 + endTextSearchPattern });

         var observation1 = ObservationStub;
         observation1.Location.Returns(projectDirectory);
         observation1.Name.Returns(filename1);

         var observation2 = ObservationStub;
         observation2.Location.Returns(projectDirectory);
         observation2.Name.Returns(filename2);

         var results = notCompiled.Execute(new List<Observation> { observation1, observation2 });

         Assert.IsTrue(results.TrueForAll(o => o.Failed == false));
      }

      [TestMethod]
      public void Execute_IgnoresBeforeSearchPattern_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { "extra" + startTextSearchPattern + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_IgnoresTextAfterEndTextSearch_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_EndTextSearchNotFound_Fails() {
         const string startTextSearchPattern = "search pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = "end pattern";

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_IgnoresCaseForFilename_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + "filename" + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns("FILENAME");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_IgnoresCaseForDirectory_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + @"directory\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);
         observation.Location.Returns("DIRECTORY");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_IgnoresCaseForFilenameInReferenceLine_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + "FILENAME" + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns("filename");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_IgnoresCaseForDirectoryInReferenceLine_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + @"DIRECTORY\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);
         observation.Location.Returns("directory");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_ActualFileIsLongerThanInConfigFile_ObservationsFails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename + "extra");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_ConfigFilenameIsLongerThanObservation_ObservationsFails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename + "extra" + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NothingAfterTheSearchPattern_ObservationsFails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns("filename");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_EmptyValueInReferenceLine_ObservationsFails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns("filename");

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingSubPathNoConfigDirectory_ObservationsPasses() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         const string subPath = "task";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + subPath + @"\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Location.Returns(subPath);
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingSubPath_ObservationsPasses() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         const string projectDirectory = "base directory";
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation(projectDirectory + "/", null) });

         const string filename = "filenname";
         const string subPath = "task";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + subPath + @"\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Location.Returns(projectDirectory + @"\" + subPath);
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_MatchingSubPathWithDoubleSlashesInReferenceFile_ObservationsPasses() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         const string projectDirectory = "base directory";
         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation(projectDirectory + "/", null) });

         const string filename = "filenname";
         const string subPath = "task";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + subPath + @"\\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Location.Returns(projectDirectory + @"\" + subPath);
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_NonMatchingSubPath_ObservationsFails() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + @"Task\" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Location.Returns("other");
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsTrue(results.First().Failed);
      }

      [TestMethod]
      public void Execute_TrimsWhitespaceBeginingOfSearchedResult_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + " " + "\t" + filename + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }

      [TestMethod]
      public void Execute_TrimsWhitespaceEndOfSearchedResult_Passes() {
         const string startTextSearchPattern = "search pattern";
         const string endTextSearchPattern = "end pattern";
         notCompiled.StartTextSearchPattern = startTextSearchPattern;
         notCompiled.EndTextSearchPattern = endTextSearchPattern;

         search.GetObservations(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<Observation> { new FileObservation("file", null) });

         const string filename = "filenname";
         search.GetReferenceLine(Arg.Any<Observation>(), Arg.Any<string>()).Returns(new List<string> { startTextSearchPattern + filename + " " + "\t" + endTextSearchPattern });

         var observation = ObservationStub;
         observation.Name.Returns(filename);

         var results = notCompiled.Execute(new List<Observation> { observation });

         Assert.IsFalse(results.First().Failed);
      }
   }
}