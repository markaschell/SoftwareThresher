using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class UsageReportTests {
      IConsole console;

      UsageReport usageReport;

      [TestInitialize]
      public void Setup() {
         console = Substitute.For<IConsole>();

         // TODO - mock this
         usageReport = new UsageReport(console, new ClassFinder());
      }

      [TestMethod]
      public void Write_Header() {
         usageReport.Write();

         console.Received().WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");
      }

      [TestMethod]
      public void Write_AtLeastOneTask() {
         usageReport.Write();

         console.Received().WriteLine("\tTask:\tFilter");
      }

      [TestMethod]
      public void Write_MultipleTasks() {
         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
      }

      [TestMethod]
      public void Write_AtLeastOneAttribute() {
         usageReport.Write();

         console.Received().WriteLine("\t\tAttribute:\tDirectory (String)");
      }

      [TestMethod]
      public void Write_AttributeWithTheCorrectTask() {
         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Filter")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("LocationSearchPattern")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("FindFilesOnDisk")));
         });
      }

      [TestMethod]
      public void Write_MultipleAttributes() {
         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Attribute")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Attribute")));
      }

      [TestMethod]
      public void Write_DoesNotContainTask() {
         usageReport.Write();

         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(":\tTask")));
      }

      [TestMethod]
      public void Write_UsageNote() {
         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("LocationSearchPattern (String) - Format is RegEx")));
      }
   }
}
