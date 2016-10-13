using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresher.Settings;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class ConfigurationLoaderTests {
      const string SettingsSectionName = "settings";
      const string TasksSectionName = "tasks";

      IConfigurationReader taskReader;

      ConfigurationLoader configurationLoader;

      [TestInitialize]
      public void Setup() {
         taskReader = Substitute.For<IConfigurationReader>();

         configurationLoader = new ConfigurationLoader(taskReader);
      }

      [TestMethod]
      public void Load_NoSettingsOrTasks() {
         var filename = "This is it";

         taskReader.GetNodes("").ReturnsForAnyArgs(new List<XmlNode>());

         configurationLoader.Load(filename);

         Received.InOrder(() => {
            taskReader.Open(filename);
            taskReader.GetNodes(SettingsSectionName);
            taskReader.GetNodes(TasksSectionName);
            taskReader.Close();
         });
      }

      [TestMethod]
      public void Load_ExceptionThrown_CloseStillCalled() {
         var exception = new Exception();
         taskReader.When(r => r.Open(Arg.Any<string>())).Do(x => { throw exception; });

         try {
            configurationLoader.Load("");
         }
         catch (Exception e) {
            Assert.AreSame(exception, e);
         }

         taskReader.Received().Close();
      }

      [TestMethod]
      public void Load_OneTaskWithNoSettings() {
         var taskType = typeof(Filter);

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         var result = configurationLoader.Load("");

         Assert.AreEqual(1, result.Tasks.Count);
         Assert.AreEqual(taskType, result.Tasks.First().GetType());
      }

      [TestMethod]
      public void Load_MultipleTasks() {
         var xmlTask = new XmlNode { Name = typeof(Filter).Name };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { xmlTask, xmlTask });

         var result = configurationLoader.Load("");

         Assert.AreEqual(2, result.Tasks.Count);
      }

      [TestMethod]
      public void Load_TaskWithOneSetting() {
         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FindFilesOnDisk).Name } });

         var result = configurationLoader.Load("");

         Assert.AreEqual(1, result.Tasks.Count);
      }

      // TODO - test with mulitple settings - no task exist yet with multiple

      [TestMethod]
      public void Load_InvalidSetting_ThrowsException() {
         var settingType = typeof(Configuration);

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(settingType.Name + " is not a supported setting.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsSettingProperty() {
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "BaseLocation", Value = "" } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         configurationLoader.Load("");
      }

      [TestMethod]
      public void Load_SetsSettingPropertyIgnoresCase() {
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "baselocation", Value = "" } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         configurationLoader.Load("");
      }

      // TODO - test with mulitple settings attributes - no setting with multiple exists yet
      
      [TestMethod]
      public void Load_InvalidSettingAttribute_ThrowsException() {
         const string invalidPropertyName = "asdf";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };

         var settingName = typeof(FileSystemSearch).Name;
         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingName, Attributes = attributes } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for " + settingName + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskHasParameterWithNoMatchingSetting_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskTypeName } });

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (ArgumentNullException e) {
            Assert.AreEqual("Search", e.ParamName);
            Assert.IsTrue(e.Message.StartsWith("The constructor for task " + taskTypeName + " has no defined setting."));
         }
      }

      [TestMethod]
      public void Load_TaskHasTwoMatchingSettings_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name },
                                                                              new XmlNode { Name = typeof(OpenGrokJsonSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskTypeName } });

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (ArgumentNullException e) {
            Assert.AreEqual("Search", e.ParamName);
            Assert.IsTrue(e.Message.StartsWith("Multiple matching settings found for task " + taskTypeName + "."));
         }
      }

      [TestMethod]
      public void Load_InvalidTaskType_ThrowsException() {
         var taskName = typeof(Configuration).Name;

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskName } });

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(taskName + " is not a supported task.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsTaskAttributes() {
         var directoryPropertyName = "Directory";
         var directoryValue = "C:/temp/";
         var searchPropertName = "SearchPattern";
         var searchValue = "*.cs";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = directoryPropertyName, Value = directoryValue },
                                                   new XmlAttribute { Name = searchPropertName, Value = searchValue } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FindFilesOnDisk).Name, Attributes = attributes } });

         var result = configurationLoader.Load("");

         var task = (FindFilesOnDisk)result.Tasks.First();
         Assert.AreEqual(directoryValue, task.Directory);
         Assert.AreEqual(searchValue, task.SearchPattern);
      }

      [TestMethod]
      public void Load_InvalidTaskAttribute_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         var invalidPropertyName = "BAD NAME";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskTypeName, Attributes = attributes } });

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for " + taskTypeName + ".", e.Message);
         }
      }

      // TODO - this test should be split into trying to get property on the base class that has a set and one for a property without a set on the child but neither of them exist yet
      // TODO - also a private attribute
      [TestMethod]
      public void Load_BaseClassAttribute_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         var reportTileTaskBaseAttribute = typeof(Task).GetProperty("ReportTitle").Name ;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = reportTileTaskBaseAttribute } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskTypeName, Attributes = attributes } });

         try {
            configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(reportTileTaskBaseAttribute + " is not a supported attribute for " + taskTypeName + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsTaskAttributeIgnoresCase() {
         var directoryPropertyName = "directory";
         var directoryValue = "C:/temp/";

         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = directoryPropertyName, Value = directoryValue } };

         taskReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FileSystemSearch).Name } });
         taskReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = typeof(FindFilesOnDisk).Name, Attributes = attributes } });

         var result = configurationLoader.Load("");

         var task = (FindFilesOnDisk)result.Tasks.First();
         Assert.AreEqual(directoryValue, task.Directory);
      }
   }
}
