using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

      [TestInitialize]
      public void Setup() {
         assemblyObjectFinder = Substitute.For<IAssemblyObjectFinder>();
         configurationReader = Substitute.For<IConfigurationReader>();
      }

      [TestMethod]
      public void Load_NoSettingsOrTasks() {
         const string filename = "This is it";

         configurationReader.GetNodes("").ReturnsForAnyArgs(new List<XmlNode>());

         new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load(filename);

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
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");
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

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         Assert.AreEqual(1, result.Tasks.Count);
         Assert.AreEqual(taskType, result.Tasks.First().GetType());
      }

      [TestMethod]
      public void Load_MultipleTasks() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name }, new XmlNode { Name = taskType.Name } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         Assert.AreEqual(2, result.Tasks.Count);
      }

      [TestMethod]
      public void Load_TaskWithMulipleSettings() {
         var settingType1 = typeof(TestSettingWithAttributes);
         var settingType2 = typeof(TestSettingNoAttributes);
         var taskType = typeof(TestTaskWithTwoSettings);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType2, settingType1 });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType1.Name }, new XmlNode { Name = settingType2.Name } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         Assert.IsTrue(((TestTaskWithTwoSettings)result.Tasks.First()).SettingsSet);
      }

      [TestMethod]
      public void Load_InvalidSetting_ThrowsException() {
         var settingType = typeof(TestTask);

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(settingType.Name + " is not a supported setting.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsSettingProperty() {
         var settingType = typeof(TestSettingWithAttributes);
         var taskType = typeof(TestTaskWithOneSetting);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         var attributeValue = "one";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "Attribute1", Value = attributeValue } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name, Attributes = attributes } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         Assert.AreEqual(attributeValue, ((TestTaskWithOneSetting)result.Tasks.First()).Attribute.Attribute1);
      }

      [TestMethod]
      public void Load_SetsMultipleSettingProperties() {
         var settingType = typeof(TestSettingWithAttributes);
         var taskType = typeof(TestTaskWithOneSetting);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });
         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         const string attributeValue1 = "one";
         const string attributeValue2 = "one";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "Attribute2", Value = attributeValue2 }, new XmlAttribute { Name = "Attribute1", Value = attributeValue1 } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name, Attributes = attributes } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         Assert.AreEqual(attributeValue1, ((TestTaskWithOneSetting)result.Tasks.First()).Attribute.Attribute1);
         Assert.AreEqual(attributeValue2, ((TestTaskWithOneSetting)result.Tasks.First()).Attribute.Attribute2);
      }

      [TestMethod]
      public void Load_InvalidSettingAttribute_ThrowsException() {
         var settingType = typeof(TestSettingWithAttributes);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });

         const string invalidPropertyName = "asdf";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };
         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name, Attributes = attributes } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for " + settingType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_SettingPrivateAttribute_ThrowsException() {
         var settingType = typeof(TestSettingWithAttributes);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });

         var privateAttribute = settingType.GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = privateAttribute } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name, Attributes = attributes } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(privateAttribute + " is not a supported attribute for " + settingType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_SettingAttributeWithNoSet_ThrowsException() {
         var settingType = typeof(TestSettingWithAttributes);

         assemblyObjectFinder.SettingTypes.Returns(new List<Type> { settingType });

         var getOnlyAttribute = settingType.GetProperty("GetOnlyAttribute").Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = getOnlyAttribute } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode> { new XmlNode { Name = settingType.Name, Attributes = attributes } });
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode>());

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(getOnlyAttribute + " is not a supported attribute for " + settingType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskHasParameterWithNoMatchingSetting_ThrowsException() {
         var taskType = typeof(TestTaskWithOneSetting);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

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

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (ArgumentNullException e) {
            Assert.AreEqual(typeof(ITestSettingWithAttributes).Name, e.ParamName);
            Assert.IsTrue(e.Message.StartsWith("Multiple matching settings found for task " + taskType.Name + "."));
         }
      }

      [TestMethod]
      public void Load_InvalidTaskType_ThrowsException() {
         var taskName = typeof(TestSettingNoAttributes).Name;

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskName } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

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

         const string attribute1Value = "one";
         const string attribute2Value = "two";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "Attribute2", Value = attribute2Value } ,
                                                   new XmlAttribute { Name = "Attribute1", Value = attribute1Value } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = attributes } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         var task = (TestTask)result.Tasks.First();
         Assert.AreEqual(attribute1Value, task.Attribute1);
         Assert.AreEqual(attribute2Value, task.Attribute2);
      }

      [TestMethod]
      public void Load_InvalidTaskAttribute_ThrowsException() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         const string invalidPropertyName = "BAD NAME";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = attributes } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for " + taskType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskPrivateAttribute_ThrowsException() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         var privateAttribute = taskType.GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = privateAttribute } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = attributes } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(privateAttribute + " is not a supported attribute for " + taskType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_TaskAttributeWithNoSet_ThrowsException() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         var reportTileTaskGetAttribute = taskType.GetProperty("ReportTitle").Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = reportTileTaskGetAttribute } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = attributes } });

         try {
            new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(reportTileTaskGetAttribute + " is not a supported attribute for " + taskType.Name + ".", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsTaskAttributeIgnoresCase() {
         var taskType = typeof(TestTask);

         assemblyObjectFinder.TaskTypes.Returns(new List<Type> { taskType });

         const string attributeValue = "value";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "attribute1", Value = attributeValue } };

         configurationReader.GetNodes(SettingsSectionName).Returns(new List<XmlNode>());
         configurationReader.GetNodes(TasksSectionName).Returns(new List<XmlNode> { new XmlNode { Name = taskType.Name, Attributes = attributes } });

         var result = new ConfigurationLoader(assemblyObjectFinder, configurationReader).Load("");

         var task = (TestTask)result.Tasks.First();
         Assert.AreEqual(attributeValue, task.Attribute1);
      }
   }
}
