using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher;
using SoftwareThresher.Configuration;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests
{
    [TestClass]
    public class TaskProcessorTests
    {
        IConfigurationLoader configurationLoader;
        Configuration configuration;
        Task task;

        IReport report;

        TaskProcessor taskProcessor;

        [TestInitialize]
        public void Setup()
        {
            configurationLoader = Substitute.For<IConfigurationLoader>();
            configuration = Substitute.For<Configuration>();
            task = Substitute.For<Task>();

            report = Substitute.For<IReport>();

            taskProcessor = new TaskProcessor(configurationLoader, report);
        }

        [TestMethod]
        public void Run_MultipleTasks()
        {
            var task2 = Substitute.For<Task>();
            var configurationnFilename = "config this";

            configurationLoader.Load(configurationnFilename).Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task, task2 });

            var passedObservation = Substitute.For<Observation>();
            passedObservation.Passed.Returns(true);
            var failedObservation = Substitute.For<Observation>();
            failedObservation.Passed.Returns(false);

            task.Execute(Arg.Is<List<Observation>>(l => l.Count == 0)).Returns(new List<Observation> { failedObservation, passedObservation });
            task2.Execute(Arg.Is<List<Observation>>(l => l.Count == 1)).Returns(new List<Observation>());

            taskProcessor.Run(configurationnFilename);


            task2.Received().Execute(Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == passedObservation));
        }

        [TestMethod]
        public void Run_ReportsErrors()
        {
            var configurationnFilename = "config this";

            configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task });

            var title = "This is it";
            task.ReportTitleForErrors.Returns(title);

            var passedObservation = Substitute.For<Observation>();
            passedObservation.Passed.Returns(true);
            var failedObservation = Substitute.For<Observation>();
            failedObservation.Passed.Returns(false);

            task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { failedObservation, passedObservation });

            taskProcessor.Run(configurationnFilename);

            Received.InOrder(() =>
            {
                report.Start(configurationnFilename);
                report.WriteResults(title, Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == failedObservation));
                report.Finialize();
            });
        }

        [TestMethod]
        public void Run_ErrorRunningTask_ReportIsFinialized()
        {
            configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task });

            var exception = new Exception();
            task.Execute(Arg.Any<List<Observation>>()).Returns(x => { throw exception; });

            try
            {
                taskProcessor.Run("");

                Assert.Fail("Should have thrown an exception");
            }
            catch (Exception e)
            {
                report.Received().Finialize();

                Assert.AreSame(exception, e);
            }
        }
    }
}
