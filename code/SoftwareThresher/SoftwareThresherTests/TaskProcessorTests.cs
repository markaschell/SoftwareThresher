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

        IReportFactory reportFactory;
        Report report;

        TaskProcessor taskProcessor;

        [TestInitialize]
        public void Setup()
        {
            configurationLoader = Substitute.For<IConfigurationLoader>();
            configuration = Substitute.For<Configuration>();
            task = Substitute.For<Task>();

            reportFactory = Substitute.For<IReportFactory>();
            report = Substitute.For<Report>();

            taskProcessor = new TaskProcessor(configurationLoader, reportFactory);
        }

        [TestMethod]
        public void Run_MultipleTasks()
        {
            var task2 = Substitute.For<Task>();

            configurationLoader.Load().Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task, task2 });

            reportFactory.Create().Returns(report);

            var passedObservation = Substitute.For<Observation>();
            passedObservation.Passed.Returns(true);
            var failedObservation = Substitute.For<Observation>();
            failedObservation.Passed.Returns(false);

            task.Execute(Arg.Is<List<Observation>>(l => l.Count == 0)).Returns(new List<Observation> { failedObservation, passedObservation });
            task2.Execute(Arg.Is<List<Observation>>(l => l.Count == 1)).Returns(new List<Observation>());

            taskProcessor.Run();

            task2.Received().Execute(Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == passedObservation));
        }

        [TestMethod]
        public void Run_ReportsErrors()
        {
            configurationLoader.Load().Returns(configuration);
            configuration.Tasks.Returns(new List<Task> { task });

            var title = "This is it";
            task.ReportTitleForErrors.Returns(title);

            reportFactory.Create().Returns(report);

            var passedObservation = Substitute.For<Observation>();
            passedObservation.Passed.Returns(true);
            var failedObservation = Substitute.For<Observation>();
            failedObservation.Passed.Returns(false);

            task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { failedObservation, passedObservation });

            taskProcessor.Run();

            report.Received().WriteResults(title, Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == failedObservation));
        }
    }
}
