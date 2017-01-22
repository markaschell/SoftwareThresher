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
      public void WriteObservationsDetails_WritesNameAndLocation() {
         const string name = "Issue";
         const string location = "It is here";
         var observation = ObservationStub;
         observation.Name.Returns(name);
         observation.Location.Returns(location);
         observation.LastEdit.Returns(Date.NullDate);

         htmlTableReport.WriteObservationsDetails(new List<Observation> { observation });

         Received.InOrder(() => {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");
            file.Write("<tr><td>" + name + "</td><td>" + location + "</td><td></td></tr>");
            file.Write("</table>");
         });
      }

      [TestMethod]
      public void WriteObservationsDetails_WritesLastEdit() {
         const string url = "url";
         var observation = ObservationStub;
         observation.LastEdit.Returns(new Date(new DateTime(2015, 1, 3)));
         observation.HistoryUrl.Returns(url);

         htmlTableReport.WriteObservationsDetails(new List<Observation> { observation });

         file.Received().Write(Arg.Is<string>(s => s.Contains("<td><a href='" + url + "'>01/03/2015</a></td>")));
      }

      [TestMethod]
      public void WriteObservationsDetails_MultipleObservations() {
         var observation = ObservationStub;

         htmlTableReport.WriteObservationsDetails(new List<Observation> { observation, observation });

         Received.InOrder(() => {
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write(Arg.Is<string>(s => s.Contains("th")));
            file.Write(Arg.Any<string>());
            file.Write(Arg.Any<string>());
            file.Write(Arg.Is<string>(s => s.Contains("table")));
         });
      }
   }
}
