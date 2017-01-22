using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Reporting;

namespace SoftwareThresherTests.Reporting {
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
   }
}
