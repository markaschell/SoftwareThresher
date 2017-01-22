using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Reporting;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting {
   [TestClass]
   public class HtmlReportBaseTests {
      ISystemFileWriter file;
      IHtmlReportData htmlReportData;

      Report htmlReportStrategy;

      [TestInitialize]
      public void Setup() {
         file = Substitute.For<ISystemFileWriter>();
         htmlReportData = Substitute.For<IHtmlReportData>();

         htmlReportStrategy = Substitute.ForPartsOf<HtmlReportBase>(file, htmlReportData);
      }

      [TestMethod]
      public void Start() {
         const string configurationFilename = "This is it";

         const string reportName = "reportName";
         htmlReportData.GetFileName(configurationFilename).Returns(reportName);

         const string startText = "This is the beginning";
         htmlReportData.StartText.Returns(startText);

         htmlReportStrategy.Start(configurationFilename);

         Received.InOrder(() => {
            file.Create(reportName);
            file.Write(startText);
         });
      }

      [TestMethod]
      public void Complete() {
         const string endText = "This is the end, my friend";
         htmlReportData.EndText.Returns(endText);

         htmlReportStrategy.Complete();

         Received.InOrder(() => {
            file.Write(endText);
            file.Received().Close();
         });
      }
   }
}
