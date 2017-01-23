using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting.Html;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting.Html {
   [TestClass]
   public class HtmlTreeReportTests {
      ISystemFileWriter file;
      IHtmlReportData htmlReportData;

      HtmlTreeReport htmlTreeReport;

      [TestInitialize]
      public void Setup() {
         file = Substitute.For<ISystemFileWriter>();
         htmlReportData = Substitute.For<IHtmlReportData>();

         htmlTreeReport = new HtmlTreeReport(file, htmlReportData);
      }
      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void WriteObservationsDetails() {
         const string name = "Issue";
         var observation = ObservationStub;
         observation.SystemSpecificString.Returns(name);
         observation.LastEdit.Returns(Date.NullDate);

         var lastEditText = "time";
         htmlReportData.GetLastEditText(observation).Returns(lastEditText);

         htmlTreeReport.WriteObservationsDetails(new List<Observation> { observation });

         Received.InOrder(() => {
            file.Write("<table id=\"resultsTable\">");
            file.Write("<tr data-depth=\"0\"><td>" + name + "</td><td>" + lastEditText + "</td></tr>");
            file.Write("</table>");
         });
      }

      [TestMethod]
      public void WriteObservationsDetails_MultipleObservations() {
         var observation = ObservationStub;

         htmlTreeReport.WriteObservationsDetails(new List<Observation> { observation, observation });

         Received.InOrder(() => {
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write(Arg.Any<string>());
            file.Write(Arg.Any<string>());
            file.Write(Arg.Is<string>(s => s.Contains("table")));
         });
      }
   }
}
