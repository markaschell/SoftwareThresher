using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresherTests.Configurations.TypeStubs;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class ConfigurationLoaderTests {
      const string SettingsSectionName = "settings";
      const string TasksSectionName = "tasks";

      IAssemblyObjectFinder assemblyObjectFinder;
      IConfigurationReader configurationReader;
      IAttributeLoader attributeLoader;

      [TestInitialize]
      public void Setup() {
         assemblyObjectFinder = Substitute.For<IAssemblyObjectFinder>();
         configurationReader = Substitute.For<IConfigurationReader>();
         attributeLoader = Substitute.For<IAttributeLoader>();
      }

      [TestMethod]
      public void Load_NoSettingsOrTasks() {
         const string filename = "This is it";

         configurationReader.GetNodes("").ReturnsForAnyArgs(new List<XmlNode>());

         new ConfigurationLoader(assemblyObjectFinder, configurationReader, null).Load(filename);

         Received.InOrder(() => {
            configurationReader.Open(filename);
            configurationReader.GetNodes(SettingsSectionName);
            configurationReader.GetNodes(TasksSectionName);
            configurationReader.Close();
         });
      }

      [TestMethod]
      public void Load_ExceptionThrown_CloseStillCalled() {
         var exception = new Exception();
         configurationReader.When(r => r.Open(Arg.Any<string>())).Do(x => { throw exception; });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader, null).Load("");
         }
         catch (Exception e) {
            Assert.AreSame(exception, e);
         }

         configurationReader.Received().Close();
      }

      [TestMethod]
      public void Load_OneTaskWithNoSettings() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         attributeLoader.SetAttributes(Arg.Any<List<XmlAttribute>>(), Arg.Any<object>()).Returns(x => x.Arg<object>());

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

         Assert.AreEqual(1, result.Tasks.Count);
         Assert.AreEqual(taskType, result.Tasks.First().GetType());
      }

      [TestMethod]
      public void Load_MultipleTasks() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name }, new XmlNode { Name = taskType.Name } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

         Assert.AreEqual(2, result.Tasks.Count);
      }

      [TestMethod]
      public void Load_TaskWithMulipleSettings() {
         var settingType1 = typeof(TestSettingWithAttributes);
         var settingType2 = typeof(TestSettingZeroAttributes);
         var taskType = typeof(TestTaskWithTwoSettings);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType2, settingType1 });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         var settingAttributes1 = new List<XmlAttribute>();
         var settingAttributes2 = new List<XmlAttribute>();
         var taskXmlNode = new XmlNode { Name = taskType.Name };
         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType1.Name, Attributes = settingAttributes1 },
                                                                                       new XmlNode { Name = settingType2.Name, Attributes = settingAttributes2 } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { taskXmlNode });

         attributeLoader.SetAttributes(Arg.Any<List<XmlAttribute>>(), Arg.Any<object>()).Returns(x => x.Arg<object>());

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

         attributeLoader.Received().SetAttributes(settingAttributes1, Arg.Any<object>());
         attributeLoader.Received().SetAttributes(settingAttributes2, Arg.Any<object>());

         Assert.IsTrue(((TestTaskWithTwoSettings)result.Tasks.First()).SettingsSet);
      }

      [TestMethod]
      public void Load_InvalidSetting_ThrowsException() {
         var settingType = typeof(TestTask);

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader, null).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(settingType.Name + " is not a supported setting.", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskHasParameterWithNoMatchingSetting_ThrowsException() {
         var taskType = typeof(TestTaskWithOneSetting);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (ArgumentNullException e) {
            Assert.AreEqual(typeof(ITestSettingWithAttributes).Name, e.ParamName);
            Assert.IsTrue(e.Message.StartsWith("The constructor for task " + taskType.Name + " has no defined setting."));
         }
      }

      [TestMethod]
      public void Load_TaskHasTwoMatchingSettings_ThrowsException() {
         var settingType = typeof(TestSettingWithAttributes);
         var taskType = typeof(TestTaskWithOneSetting);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name },
                                                                                       new XmlNode { Name = settingType.Name } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         attributeLoader.SetAttributes(Arg.Any<List<XmlAttribute>>(), Arg.Any<object>()).Returns(x => x.Arg<object>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (ArgumentNullException e) {
            Assert.AreEqual(typeof(ITestSettingWithAttributes).Name, e.ParamName);
            Assert.IsTrue(e.Message.StartsWith("Multiple matching settings found for task " + taskType.Name + "."));
         }
      }

      [TestMethod]
      public void Load_InvalidTaskType_ThrowsException() {
         var taskName = typeof(TestSettingZeroAttributes).Name;

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskName } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(taskName + " is not a supported task.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsTaskAttributes() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         var taskAttributes = new List<XmlAttribute>();
         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = taskAttributes } });

         var taskWithAttributesSet = new TestTask();
         attributeLoader.SetAttributes(taskAttributes, Arg.Any<object>()).Returns(taskWithAttributesSet);

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader, attributeLoader).Load("");

         Assert.AreSame(taskWithAttributesSet, (TestTask)result.Tasks.First());
      }
   }
}
