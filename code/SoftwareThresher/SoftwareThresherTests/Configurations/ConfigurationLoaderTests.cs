using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class ConfigurationLoaderTests {
      ITaskReader taskReader;

      ConfigurationLoader configurationLoader;

      [TestInitialize]
      public void Setup() {
         taskReader = Substitute.For<ITaskReader>();

         configurationLoader = new ConfigurationLoader(taskReader);
      }

      [TestMethod]
      public void Start_NoTasks() {
         var filename = "This is it";

         configurationLoader.Load(filename);

         Received.InOrder(() => {
            taskReader.Open(filename);
            taskReader.GetNextTask();
            taskReader.Close();
         });
      }

      [TestMethod]
      public void Start_OpenThrowsException_CloseStillCalled() {
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
      public void Start_GetNextTaskThrowsException_CloseStillCalled() {
         var exception = new Exception();
         taskReader.When(r => r.GetNextTask()).Do(x => { throw exception; });

         try {
            configurationLoader.Load("");
         }
         catch (Exception e) {
            Assert.AreSame(exception, e);
         }

         taskReader.Received().Close();
      }

      [TestMethod]
      public void Start_OneTask() {
         var taskType = typeof(FindFilesOnDisk);
         taskReader.GetNextTask().Returns(new XmlTask { Name = taskType.Name }, (XmlTask)null);

         var result = configurationLoader.Load("");

         Assert.AreEqual(1, result.Tasks.Count);
         Assert.AreEqual(taskType, result.Tasks.First().GetType());
      }

      [TestMethod]
      public void Start_MultipleTasks() {
         var xmlTask = new XmlTask { Name = typeof(FindFilesOnDisk).Name };
         taskReader.GetNextTask().Returns(xmlTask, xmlTask, null);

         var result = configurationLoader.Load("");

         taskReader.Received(3).GetNextTask();

         Assert.AreEqual(2, result.Tasks.Count);
      }

      [TestMethod]
      public void Start_InvalidTaskType_ThrowsException() {
         var taskType = typeof(Configuration);
         taskReader.GetNextTask().Returns(new XmlTask { Name = taskType.Name });

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(taskType.Name + " is not a supported task type.", e.Message);
         }
      }

      [TestMethod]
      public void Start_SetsProperties() {
         var directoryPropertyName = "Directory";
         var locationValue = "C:/temp/";
         var searchPropertName = "SearchPattern";
         var searchValue = "*.cs";
         var attributes = new Dictionary<string, string> { { directoryPropertyName, locationValue }, { searchPropertName, searchValue } };
         taskReader.GetNextTask().Returns(new XmlTask { Name = typeof(FindFilesOnDisk).Name, Attributes = attributes }, (XmlTask)null);

         var result = configurationLoader.Load("");

         var task = (FindFilesOnDisk)result.Tasks.First();
         Assert.AreEqual(locationValue, task.Directory);
         Assert.AreEqual(searchValue, task.SearchPattern);
      }

      [TestMethod]
      public void Start_InvalidAttribute_ThrowsException() {
         var taskTypeName = typeof(FindFilesOnDisk).Name;

         var invalidPropertyName = "BAD NAME";
         var attributes = new Dictionary<string, string> { { invalidPropertyName, "" } };
         taskReader.GetNextTask().Returns(new XmlTask { Name = taskTypeName, Attributes = attributes });

         try {
            var result = configurationLoader.Load("");

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for task type " + taskTypeName + ".", e.Message);
         }
      }

      [TestMethod]
      public void Start_SetsPropertyIgnoresCase() {
         var taskType = typeof(FindFilesOnDisk);

         var directoryPropertyName = "directory";
         var locationValue = "C:/temp/";
         var attributes = new Dictionary<string, string> { { directoryPropertyName, locationValue } };
         taskReader.GetNextTask().Returns(new XmlTask { Name = taskType.Name, Attributes = attributes }, (XmlTask)null);

         var result = configurationLoader.Load("");

         var task = (FindFilesOnDisk)result.Tasks.First();
         Assert.AreEqual(locationValue, task.Directory);
      }
   }
}
