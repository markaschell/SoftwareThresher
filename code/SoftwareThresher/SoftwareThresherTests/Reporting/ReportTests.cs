using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting
{
    [TestClass]
    public class ReportTests
    {
        ISystemFile file;
        IReportData reportData;

        Report report;

        [TestInitialize]
        public void Setup()
        {
            file = Substitute.For<ISystemFile>();
            reportData = Substitute.For<IReportData>();

            report = new Report(file, reportData);
        }

        [TestMethod]
        public void Start()
        {
            var configurationFilename = "This is it";

            var reportName = "reportName";
            var time = "wakie wakie";
            reportData.GetFileNameWithoutExtesion(configurationFilename).Returns(reportName);
            reportData.GetTimestamp().Returns(time);

            report.Start(configurationFilename);

            Received.InOrder(() =>
            {
                file.Create(reportName + ".html");
                file.Write("<html><head></head><body>");
                file.Write(time);
                file.Write("");
            });
        }

        [TestMethod]
        public void WriteResults_WritesHeader()
        {
            var header = "This is my stupid title";
            var observation = Substitute.For<Observation>();

            report.WriteResults(header, new List<Observation> { observation }, 3);

            file.Received().Write("<h3>" + header + "</h3> (1 of 3)");
        }

        [TestMethod]
        public void WriteResults_ObservationData()
        {
            var name = "Issue";
            var location = "It is here";
            var observation = Substitute.For<Observation>();
            observation.Name.Returns(name);
            observation.Location.Returns(location);

            report.WriteResults("", new List<Observation> { observation }, 0);

            file.Received().Write(name + " - " + location);
        }

        [TestMethod]
        public void WriteResults_MultipleObservations()
        {
            var observation = Substitute.For<Observation>();

            report.WriteResults("", new List<Observation> { observation, observation }, 0);

            file.Received(4).Write(Arg.Any<string>());
        }

        [TestMethod]
        public void WriteResults_NoItems_NothingIsWritten()
        {
            report.WriteResults("testing", new List<Observation>(), 0);

            file.DidNotReceive().Write(Arg.Any<string>());
        }

        [TestMethod]
        public void Complete()
        {
            var time = "wakie wakie";
            reportData.GetTimestamp().Returns(time);

            report.Complete();

            Received.InOrder(() =>
            {
                file.Write(time);
                file.Write("</body></html>");
                file.Received().Close();
            });
        }
    }
}
