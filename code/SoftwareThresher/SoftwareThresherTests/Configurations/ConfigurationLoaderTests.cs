using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Configurations;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests.Configurations
{
    [TestClass]
    public class ConfigurationLoaderTests
    {
        IXmlDocumentReader xmlDocumentReader;

        ConfigurationLoader configurationLoader;

        [TestInitialize]
        public void Setup()
        {
            xmlDocumentReader = Substitute.For<IXmlDocumentReader>();

            configurationLoader = new ConfigurationLoader(xmlDocumentReader);
        }

        [TestMethod]
        public void Start_NoTasks()
        {
            var filename = "This is it";

            configurationLoader.Load(filename);

            Received.InOrder(() =>
            {
                xmlDocumentReader.Open(filename);
                xmlDocumentReader.GetNextTask();
                xmlDocumentReader.Close();
            });
        }

        [TestMethod]
        public void Start_OneTask()
        {
            var taskType = typeof(FindWindowsFiles);
            xmlDocumentReader.GetNextTask().Returns(new XmlTask { Name = taskType.Name }, (XmlTask)null);

            var result = configurationLoader.Load("");

            Assert.AreEqual(1, result.Tasks.Count);
            Assert.AreEqual(taskType, result.Tasks.First().GetType());
        }

        [TestMethod]
        public void Start_MultipleTasks()
        {
            var xmlTask = new XmlTask { Name = typeof(FindWindowsFiles).Name };
            xmlDocumentReader.GetNextTask().Returns(xmlTask, xmlTask, null);

            var result = configurationLoader.Load("");

            xmlDocumentReader.Received(3).GetNextTask();

            Assert.AreEqual(2, result.Tasks.Count);
        }

        [TestMethod]
        public void Start_InvalidTaskType_ThrowsException()
        {
            var taskType = typeof(Configuration);
            xmlDocumentReader.GetNextTask().Returns(new XmlTask { Name = taskType.Name });

            try
            {
                var result = configurationLoader.Load("");

                Assert.Fail("Should have thrown an exception.");
            }
            catch (NotSupportedException e)
            {
                Assert.AreEqual(taskType.Name + " is not a supported task type.", e.Message);
            }
        }
    }
}
