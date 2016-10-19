using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresher.Utilities;
using SoftwareThresherTests.Configurations.TypeStubs;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class UsageReportTests {
      IClassFinder classFinder;
      IConsole console;

      UsageReport usageReport;

      [TestInitialize]
      public void Setup() {
         classFinder = Substitute.For<IClassFinder>();
         console = Substitute.For<IConsole>();

         usageReport = new UsageReport(classFinder, console);
      }

      [TestMethod]
      public void Write_Header() {
         classFinder.TaskTypes.Returns(new List<Type>());

         usageReport.Write();

         console.Received().WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");
      }

      [TestMethod]
      public void Write_AtLeastOneTask() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\tTask:\tTestTask");
      }

      [TestMethod]
      public void Write_MultipleTasks() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask), typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
      }

      [TestMethod]
      public void Write_AtLeastOneAttribute() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\t\tAttribute:\tAttribute1 (String)");
      }

      [TestMethod]
      public void Write_PrivateAttribute_DoesNotWriteIt() {
         var taskType = typeof(TestTask);

         classFinder.TaskTypes.Returns(new List<Type> { taskType });

         usageReport.Write();

         var privateAttribute = taskType.GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(privateAttribute)));
      }

      [TestMethod]
      public void Write_GetOnlyAttribute_DoesNotWriteIt() {
         var taskType = typeof(TestTask);

         classFinder.TaskTypes.Returns(new List<Type> { taskType });

         usageReport.Write();

         var getOnlyAttribute = taskType.GetProperty("ReportTitle").Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(getOnlyAttribute)));
      }

      [TestMethod]
      public void Write_AttributeWithTheCorrectTask() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask), typeof(TestTaskWithOneSetting) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTask")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTaskWithOneSetting")));
         });
      }

      [TestMethod]
      public void Write_MultipleAttributes() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Attribute2")));
      }

      [TestMethod]
      public void Write_UsageNote() {
         classFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("AttributeWithNote (String) - Note")));
      }
   }
}
