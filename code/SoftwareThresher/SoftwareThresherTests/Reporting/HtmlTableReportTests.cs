using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting {
   [TestClass]
   public class HtmlTableReportTests {
      ISystemFileWriter file;
      IHtmlReportData htmlReportData;

      HtmlTableReport htmlTableReport;

      [TestInitialize]
      public void Setup() {
         file = Substitute.For<ISystemFileWriter>();
         htmlReportData = Substitute.For<IHtmlReportData>();

         htmlTableReport = new HtmlTableReport(file, htmlReportData);
      }

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void Start() {
         const string configurationFilename = "This is it";

         const string reportName = "reportName";
         htmlReportData.GetFileName(configurationFilename).Returns(reportName);

         const string startText = "This is the beginning";
         htmlReportData.StartText.Returns(startText);

         htmlTableReport.Start(configurationFilename);

         Received.InOrder(() => {
            file.Create(reportName);
            file.Write(startText);
         });
      }

      [TestMethod]
      public void WriteObservations_WritesHeader() {
         const string header = "This is my stupid title";

         const string newLine = "new";
         htmlReportData.NewLine.Returns(newLine);

         htmlTableReport.WriteObservations(header, 1, 0, new TimeSpan(9, 7, 5, 3, 1), new List<Observation>());

         Received.InOrder(() => {
            file.Write("<h3 style=\"display: inline;\">" + header + ": 1</h3> in 9.07:05:03.0010000" + newLine);
            file.Write(newLine);
         });
      }

      [TestMethod]
      public void WriteObservations_WritesAbsoluteValueNumberOfChanges() {
         htmlTableReport.WriteObservations(string.Empty, -2, 0, new TimeSpan(9, 7, 5, 3, 1), new List<Observation>());

         file.Write(Arg.Is<string>(s => s.Contains(": 2</h3>")));
      }

      [TestMethod]
      public void WriteObservations_WritesNameAndLocation() {
         const string name = "Issue";
         const string location = "It is here";
         var observation = ObservationStub;
         observation.Name.Returns(name);
         observation.Location.Returns(location);
         observation.LastEdit.Returns(Date.NullDate);

         htmlTableReport.WriteObservations("", 1, 0, new TimeSpan(), new List<Observation> { observation });


         Received.InOrder(() => {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");
            file.Write("<tr><td>" + name + "</td><td>" + location + "</td><td></td></tr>");
            file.Write("</table>");
         });
      }

      [TestMethod]
      public void WriteObservations_WritesLastEdit() {
         const string url = "url";
         var observation = ObservationStub;
         observation.LastEdit.Returns(new Date(new DateTime(2015, 1, 3)));
         observation.HistoryUrl.Returns(url);

         htmlTableReport.WriteObservations("", 1, 0, new TimeSpan(), new List<Observation> { observation });

         file.Received().Write("<tr><td></td><td></td><td><a href='" + url + "'>01/03/2015</a></td></tr>");
      }

      [TestMethod]
      public void WriteObservations_MultipleObservations() {
         var observation = ObservationStub;
         observation.LastEdit.Returns(Date.NullDate);

         const string newLine = "new";
         htmlReportData.NewLine.Returns(newLine);

         htmlTableReport.WriteObservations("", 1, 0, new TimeSpan(), new List<Observation> { observation, observation });

         Received.InOrder(() => {
            file.Write(Arg.Is<string>(s => s.StartsWith("<h3")));
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write(Arg.Is<string>(s => s.Contains("th")));
            file.Write(Arg.Any<string>());
            file.Write(Arg.Any<string>());
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write(newLine);
         });
      }

      [TestMethod]
      public void WriteObservations_NothingAddedOrFailed_NothingIsWritten() {
         var observation = ObservationStub;

         htmlTableReport.WriteObservations("testing", 0, 0, new TimeSpan(), new List<Observation> { observation });

         file.DidNotReceive().Write(Arg.Any<string>());
      }

      [TestMethod]
      public void WriteObservations_NoThingFailed_TableIsNotWritten() {
         htmlTableReport.WriteObservations("testing", 1, 0, new TimeSpan(), new List<Observation>());

         file.DidNotReceive().Write(Arg.Is<string>(s => s.Contains("table")));
      }

      [TestMethod]
      public void Complete() {
         const string endText = "This is the end, my friend";
         htmlReportData.EndText.Returns(endText);

         htmlTableReport.Complete();

         Received.InOrder(() => {
            file.Write(endText);
            file.Received().Close();
         });
      }
   }
}
