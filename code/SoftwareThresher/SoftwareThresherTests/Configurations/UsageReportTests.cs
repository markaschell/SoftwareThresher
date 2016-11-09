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
      IAssemblyObjectFinder assemblyObjectFinder;
      IConsole console;

      UsageReport usageReport;

      [TestInitialize]
      public void Setup() {
         assemblyObjectFinder = Substitute.For<IAssemblyObjectFinder>();
         console = Substitute.For<IConsole>();

         usageReport = new UsageReport(assemblyObjectFinder, console);
      }

      [TestMethod]
      public void Write_Header() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type>());

         usageReport.Write();

         console.Received().WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");
      }

      [TestMethod]
      public void Write_AtLeastOneSettingType() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine("\tSetting Type:\tITestSettingWithAttributes");
      }

      [TestMethod]
      public void Write_MultipleSettingTypes() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes), typeof(ITestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingWithAttributes")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingWithAttributes")));
      }

      [TestMethod]
      public void Write_MultipleSettingTypesInAlphabeticalOrder() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes), typeof(ITestSettingNoAttributes) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingNoAttributes")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingWithAttributes")));
         });
      }

      [TestMethod]
      public void Write_SettingTypeWithOneImplementation() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine("\t\tSetting:\tTestSettingWithAttributes");
      }

      [TestMethod]
      public void Write_SettingImplementationWithNote() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingNoAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithNote) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("TestSettingWithNote - Note")));
      }

      [TestMethod]
      public void Write_SettingTypeWithMultipleImplementations() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes), typeof(TestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("TestSettingWithAttributes")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("TestSettingWithAttributes")));
      }

      [TestMethod]
      public void Write_SettingTypeWithMultipleImplementationsInAlphabeticalOrder() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes), typeof(TestSettingOneAttribute) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestSettingOneAttribute")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("\tTestSettingWithAttributes")));
         });
      }

      [TestMethod]
      public void Write_SettingTypeIsNotInterfaceForSetting_DoesNotWriteIt() {
         var settingToNotMatch = typeof(TestSettingZeroAttributes);

         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingToNotMatch });

         usageReport.Write();

         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(settingToNotMatch.Name)));
      }

      [TestMethod]
      public void Write_SettingWithAtLeastOneAttribute() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine("\t\t\tAttribute:\t\tAttribute1 (String)");
      }

      [TestMethod]
      public void Write_SettingWithPrivateAttribute_DoesNotWriteIt() {
         var settingType = typeof(TestSettingWithAttributes);

         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });

         usageReport.Write();

         var privateAttribute = settingType.GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(privateAttribute)));
      }

      [TestMethod]
      public void Write_SettingWithGetOnlyAttribute_DoesNotWriteIt() {
         var settingType = typeof(TestSettingWithAttributes);

         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });

         usageReport.Write();

         var getOnlyAttribute = settingType.GetProperty("GetOnlyAttribute").Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(getOnlyAttribute)));
      }

      [TestMethod]
      public void Write_AttributeWithTheCorrectSettingAndSettingType() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes), typeof(ITestSettingNoAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingNoAttributes")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingWithAttributes")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestSettingWithAttributes")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
         });
      }

      [TestMethod]
      public void Write_SettingWithMultipleAttributes_DisplaysInAlphabeticalOrder() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute2")));
         });
      }

      [TestMethod]
      public void Write_SettingWithMultipleAttributes_DisplaysOptionalInAlphabeticalOrder() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr2")));
         });
      }

      [TestMethod]
      public void Write_SettingWithMultipleAttributes_DisplaysOptionalAfterRequired() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr1")));
         });
      }

      [TestMethod]
      public void Write_SettingAttributeUsageNote() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { typeof(TestSettingWithAttributes) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("AttributeWithNote (String) - Note")));
      }

      [TestMethod]
      public void Write_AtLeastOneTask() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\tTask:\tTestTask()");
      }

      [TestMethod]
      public void Write_TaskWithUsageNote() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTaskWithNote) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("TestTaskWithNote() - Note")));
      }

      [TestMethod]
      public void Write_MultipleTasks() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask), typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
         console.Received().WriteLine(Arg.Is<string>(s => s.Contains("Task")));
      }

      [TestMethod]
      public void Write_MultipleTasks_DisplaysInAlphabeticalOrder() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTaskWithTwoSettings), typeof(TestTask) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Task")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTaskWithTwoSettings")));
         });
      }

      [TestMethod]
      public void Write_TaskWithOneSetting() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTaskWithOneSetting) });

         usageReport.Write();

         console.Received().WriteLine("\tTask:\tTestTaskWithOneSetting(ITestSettingWithAttributes)");
      }

      [TestMethod]
      public void Write_TaskWithMultipleSettings_DisplaysInAlphabeticalOrder() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTaskWithTwoSettings) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(l => l.EndsWith("(ITestSettingNoAttributes, ITestSettingWithAttributes)")));
      }

      [TestMethod]
      public void Write_TaskWithAtLeastOneAttribute() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\t\tAttribute:\t\tAttribute1 (String)");
      }

      [TestMethod]
      public void Write_TaskWithOptionalAttribute() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\t\tOptional Attribute:\tAaOptionalAttr1 (String)");
      }

      [TestMethod]
      public void Write_TaskBaseAttribute() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine("\t\tOptional Attribute:\tReportHeaderText (String)");
      }

      [TestMethod]
      public void Write_TaskWithPrivateAttribute_DoesNotWriteIt() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         usageReport.Write();

         var privateAttribute = taskType.GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(privateAttribute)));
      }

      [TestMethod]
      public void Write_TaskWithGetOnlyAttribute_DoesNotWriteIt() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         usageReport.Write();

         var getOnlyAttribute = taskType.GetProperty("GetOnlyAttribute").Name;
         console.DidNotReceive().WriteLine(Arg.Is<string>(s => s.Contains(getOnlyAttribute)));
      }

      [TestMethod]
      public void Write_AttributeWithTheCorrectTask() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask), typeof(TestTaskWithOneSetting) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTask")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTaskWithOneSetting")));
         });
      }

      [TestMethod]
      public void Write_TaskWithMultipleAttributes_DisplaysInAlphabeticalOrder() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute2")));
         });
      }

      [TestMethod]
      public void Write_TaskWithMultipleAttributes_DisplaysOptionalInAlphabeticalOrder() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr2")));
         });
      }

      [TestMethod]
      public void Write_TaskWithMultipleAttributes_DisplaysOptionalAfterRequired() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("Attribute1")));
            console.WriteLine(Arg.Is<string>(s => s.Contains("AaOptionalAttr2")));
         });
      }

      [TestMethod]
      public void Write_TaskAttributeUsageNote() {
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         console.Received().WriteLine(Arg.Is<string>(s => s.EndsWith("AttributeWithNote (String) - Note")));
      }

      [TestMethod]
      public void Write_SettingsAndTasks() {
         assemblyObjectFinder.SettingInterfaces.Returns(new List<Type> { typeof(ITestSettingWithAttributes) });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { typeof(TestTask) });

         usageReport.Write();

         Received.InOrder(() => {
            console.WriteLine(Arg.Is<string>(s => s.Contains("ITestSettingWithAttributes")));
            console.WriteLine(Arg.Is<string>(s => s == string.Empty));
            console.WriteLine(Arg.Is<string>(s => s.Contains("TestTask")));
         });
      }
   }
}
