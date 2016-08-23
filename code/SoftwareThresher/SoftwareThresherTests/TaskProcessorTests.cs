using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher;
using SoftwareThresher.Configuration;
using SoftwareThresher.Observations;
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
        public void RunMultipleTasks()
        {
            var task2 = Substitute.For<Task>();

            configurationLoader.Load().Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task, task2 });

            var task1Observations = new List<Observation>();
            task.Execute(Arg.Is<List<Observation>>(l => l.Count == 0)).Returns(task1Observations);

            taskProcessor.Run();

            task2.Received().Execute(task1Observations);
        }
    }
}
