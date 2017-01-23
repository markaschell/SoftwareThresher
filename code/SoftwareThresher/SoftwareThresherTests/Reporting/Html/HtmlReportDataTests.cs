using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Reporting.Html;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting.Html {
   [TestClass]
   public class HtmlReportDataTests {
      IReportData reportData;

      HtmlReportData htmlReportData;

      [TestInitialize]
      public void Setup() {
         reportData = Substitute.For<IReportData>();

         htmlReportData = new HtmlReportData(reportData);
      }

      [TestMethod]
      public void Start() {
         const string configurationFilename = "This is it";

         const string reportName = "reportName";
         reportData.GetFileNameWithoutExtesion(configurationFilename).Returns(reportName);

         var result = htmlReportData.GetFileName(configurationFilename);

         Assert.AreEqual(reportName + ".html", result);
      }

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void GetLastEditText_ReturnsLastEditDateLinkedToHistoryUrl() {
         const string url = "url";
         var observation = ObservationStub;
         observation.LastEdit.Returns(new Date(new DateTime(2015, 1, 3)));
         observation.HistoryUrl.Returns(url);

         var result = htmlReportData.GetLastEditText(observation);

         Assert.AreEqual("<a href='" + url + "'>01/03/2015</a>", result);
      }

      [TestMethod]
      public void GetLastEditText_NullDate_ReturnsEmptyString() {
         var observation = ObservationStub;
         observation.LastEdit.Returns(Date.NullDate);

         var result = htmlReportData.GetLastEditText(observation);

         Assert.AreEqual(string.Empty, result);
      }

   }
}
