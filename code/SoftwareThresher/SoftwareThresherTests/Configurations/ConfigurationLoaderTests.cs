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
            var xmlTask = new XmlTask { Name = "FindWindowsFiles" };
            xmlDocumentReader.GetNextTask().Returns(xmlTask, (XmlTask)null);

            var result = configurationLoader.Load("");

            Assert.AreEqual(1, result.Tasks.Count);
            Assert.AreEqual(typeof(FindWindowsFiles), result.Tasks.First().GetType());
        }

        [TestMethod]
        public void Start_MultipleTasks()
        {
            var xmlTask = new XmlTask { Name = "FindWindowsFiles" };
            xmlDocumentReader.GetNextTask().Returns(xmlTask, xmlTask, null);

            var result = configurationLoader.Load("");

            xmlDocumentReader.Received(3).GetNextTask();

            Assert.AreEqual(2, result.Tasks.Count);
        }
    }
}
