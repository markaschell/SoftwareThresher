using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting.Html;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting.Html {
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
      public void WriteObservationsDetails() {
         const string name = "Issue";
         const string location = "It is here";
         var observation = ObservationStub;
         observation.Name.Returns(name);
         observation.Location.Returns(location);

         var lastEditText = "time";
         htmlReportData.GetLastEditText(observation).Returns(lastEditText);

         htmlTableReport.WriteObservationsDetails(new List<Observation> { observation });

         Received.InOrder(() => {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");
            file.Write("<tr><td>" + name + "</td><td>" + location + "</td><td>" + lastEditText + "</td></tr>");
            file.Write("</table>");
         });
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
