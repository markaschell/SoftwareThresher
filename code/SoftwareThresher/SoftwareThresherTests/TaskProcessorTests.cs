using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher;
using SoftwareThresher.Configuration;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests
{
    [TestClass]
    public class TaskProcessorTests
    {
        IConfigurationLoader configurationLoader;
        Configuration configuration;
        Task task;

        TaskProcessor taskProcessor;

        [TestInitialize]
        public void Setup()
        {
            configurationLoader = Substitute.For<IConfigurationLoader>();
            configuration = Substitute.For<Configuration>();
            task = Substitute.For<Task>();

            taskProcessor = new TaskProcessor(configurationLoader);
        }

        [TestMethod]
        public void RunOneTask()
        {
            configurationLoader.Load().Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task });

            taskProcessor.Run();

            task.Received().Execute();
        }
    }
}
