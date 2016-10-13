using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Settings;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Settings {
   [TestClass]
   public class FileSystemSearchTests {
      ISystemFileReader reader;

      FileSystemSearch search;

      [TestInitialize]
      public void Setup() {
         reader = Substitute.For<ISystemFileReader>();

         search = new FileSystemSearch(reader);
      }

      [TestMethod]
      public void GetReferencesInFile_OpensReadsAndClosesFile() {
         var filename = "filename";
         var searchText = "find it";

         reader.ReadLine().Returns((string)null);

         search.GetReferencesInFile(filename, searchText);

         Received.InOrder(() => {
            reader.Open(filename);
            reader.ReadLine();
            reader.Close();
         });
      }

      [TestMethod]
      public void GetReferencesInFile_ExceptionOnOpen_StillCallsClose() {
         var exception = new Exception();
         reader.When(r => r.Open(Arg.Any<string>())).Do(x => { throw exception; });

         try {
            search.GetReferencesInFile("", "");
         }
         catch (Exception e) {
            Assert.AreSame(exception, e);
         }

         reader.Received().Close();
      }

      [TestMethod]
      public void GetReferencesInFile_ExceptionOnReadLine_StillCallsClose() {
         var exception = new Exception();
         reader.When(r => r.ReadLine()).Do(x => { throw exception; });

         try {
            search.GetReferencesInFile("", "");
         }
         catch (Exception e) {
            Assert.AreSame(exception, e);
         }

         reader.Received().Close();
      }

      [TestMethod]
      public void GetReferencesInFile_ReadsUntilEndOfFile() {
         reader.ReadLine().Returns("DEC", "HEX", null);

         search.GetReferencesInFile("", "");

         reader.Received(3).ReadLine();
      }

      [TestMethod]
      public void GetReferencesInFile_PatternMatches_LineReturned() {
         var pattern = "Test";
         var line = "kjsdf" + pattern + "kjasf";

         reader.ReadLine().Returns(line, (string)null);

         var results = search.GetReferencesInFile("", pattern);

         Assert.AreEqual(1, results.Count);
         Assert.AreEqual(line, results.First());
      }

      [TestMethod]
      public void GetReferencesInFile_PatternMatchesMultiple_LinesReturned() {
         var pattern = "Test";
         var line1 = "kjsdf" + pattern + "kjasf";
         var line2 = pattern;

         reader.ReadLine().Returns(line1, line2, null);

         var results = search.GetReferencesInFile("", pattern);

         Assert.AreEqual(2, results.Count);
      }

      [TestMethod]
      public void GetReferencesInFile_PatternDoesNotMatches_LineNotReturned() {
         reader.ReadLine().Returns("Other", (string)null);

         var results = search.GetReferencesInFile("", "Test");

         Assert.AreEqual(0, results.Count);
      }
   }
}
