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

         taskReader.GetSettings().Returns(new List<XmlSetting>());
         taskReader.GetTasks().Returns(new List<XmlTask>());

         configurationLoader.Load(filename);

         Received.InOrder(() => {
            taskReader.Open(filename);
            taskReader.GetSettings();
            taskReader.GetTasks();
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

         taskReader.GetSettings().Returns(new List<XmlSetting>());
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskType.Name } });

         var result = configurationLoader.Load("");

         Assert.AreEqual(1, result.Tasks.Count);
         Assert.AreEqual(taskType, result.Tasks.First().GetType());
      }

      [TestMethod]
      public void Load_MultipleTasks() {
         var xmlTask = new XmlTask { Name = typeof(Filter).Name };

         taskReader.GetSettings().Returns(new List<XmlSetting>());
         taskReader.GetTasks().Returns(new List<XmlTask> { xmlTask, xmlTask });

         var result = configurationLoader.Load("");

         Assert.AreEqual(2, result.Tasks.Count);
      }

      [TestMethod]
      public void Load_TaskWithOneSetting() {
         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = typeof(FindFilesOnDisk).Name } });

         var result = configurationLoader.Load("");

         Assert.AreEqual(1, result.Tasks.Count);
      }

      // TODO - test with mulitple settings - no task exist yet with multiple

      [TestMethod]
      public void Load_InvalidSettingType_ThrowsException() {
         var settingType = typeof(Configuration);
         var taskType = typeof(FindFilesOnDisk);

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = typeof(Search).Name, Type = settingType.Name } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(settingType.Name + " is not a supported setting type.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SettingTypeIsSettingName_ThrowsException() {
         var settingName = typeof(Search).Name;
         var taskType = typeof(FindFilesOnDisk);

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = settingName, Type = settingName } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(settingName + " is not a supported setting type.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsSettingProperty() {
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "BaseLocation", Value = "" } };

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = typeof(Search).Name, Type = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         var result = configurationLoader.Load("");
      }

      [TestMethod]
      public void Load_SetsSettingPropertyIgnoresCase() {
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "baselocation", Value = "" } };

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = typeof(Search).Name, Type = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         var result = configurationLoader.Load("");
      }

      // TODO - test with mulitple settings attributes - no setting with multiple exists yet

      [TestMethod]
      public void Load_InvalidSettingName_ThrowsException() {
         var settingName = "klsadf";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "baselocation", Value = "" } };

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = settingName, Type = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual("Unsupported type " + settingName + ".", e.Message);
         }
      }

      //[TestMethod]
      //public void Load_SettingNameHasTypeName_ThrowsException() {
      //   var settingTypeName = typeof(FileSystemSearch).Name;
      //   var taskTypeName = typeof(FindFilesOnDisk).Name;

      //   taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = settingTypeName, Type = settingTypeName } });
      //   taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskTypeName } });

      //   try {
      //      var result = configurationLoader.Load("");

      //      Assert.Fail("Should have thrown an exception.");
      //   }
      //   catch (ArgumentNullException e) {
      //      Assert.AreEqual("Search", e.ParamName);
      //      Assert.IsTrue(e.Message.StartsWith("The constructor for task " + taskTypeName + " has no defined setting."));
      //   }
      //}

      [TestMethod]
      public void Load_InvalidSettingAttribute_ThrowsException() {
         var invalidPropertyName = "asdf";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };

         var settingName = typeof(Search).Name;
         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = settingName, Type = typeof(FileSystemSearch).Name, Attributes = attributes } });
         taskReader.GetTasks().Returns(new List<XmlTask>());

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for type " + settingName + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskHasParameterWithNoMatchingSetting_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         taskReader.GetSettings().Returns(new List<XmlSetting>());
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskTypeName } });

         try {
            var result = configurationLoader.Load("");

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

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name },
                                                                 new XmlSetting { Name = "Search", Type = typeof(OpenGrokJsonSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskTypeName } });

         try {
            var result = configurationLoader.Load("");

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

         taskReader.GetSettings().Returns(new List<XmlSetting>());
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskName } });

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual("Unsupported type " + taskName + ".", e.Message);
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

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = typeof(FindFilesOnDisk).Name, Attributes = attributes } });

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

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskTypeName, Attributes = attributes } });

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for type " + taskTypeName + ".", e.Message);
         }
      }

      // TODO - this test should be split into trying to get property on the base class that has a set and one for a property without a set on the child but neither of them exist yet
      // TODO - also a private attribute
      [TestMethod]
      public void Load_BaseClassAttribute_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         var reportTileTaskBaseAttribute = typeof(Task).GetProperty("ReportTitle").Name ;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = reportTileTaskBaseAttribute } };

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = taskTypeName, Attributes = attributes } });

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(reportTileTaskBaseAttribute + " is not a supported attribute for type " + taskTypeName + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsTaskAttributeIgnoresCase() {
         var directoryPropertyName = "directory";
         var directoryValue = "C:/temp/";

         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = directoryPropertyName, Value = directoryValue } };

         taskReader.GetSettings().Returns(new List<XmlSetting> { new XmlSetting { Name = "Search", Type = typeof(FileSystemSearch).Name } });
         taskReader.GetTasks().Returns(new List<XmlTask> { new XmlTask { Name = typeof(FindFilesOnDisk).Name, Attributes = attributes } });

         var result = configurationLoader.Load("");

         var task = (FindFilesOnDisk)result.Tasks.First();
         Assert.AreEqual(directoryValue, task.Directory);
      }
   }
}
