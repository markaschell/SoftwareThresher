using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting.Html;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting.Html {
   [TestClass]
   public class HtmlReportBaseTests {
      ISystemFileWriter file;
      IHtmlReportData htmlReportData;

      HtmlReportBase htmlReportBase;

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestInitialize]
      public void Setup() {
         file = Substitute.For<ISystemFileWriter>();
         htmlReportData = Substitute.For<IHtmlReportData>();

         htmlReportBase = Substitute.ForPartsOf<HtmlReportBase>(file, htmlReportData);
      }

      [TestMethod]
      public void Start() {
         const string configurationFilename = "This is it";

         const string reportName = "reportName";
         htmlReportData.GetFileName(configurationFilename).Returns(reportName);

         const string startText = "This is the beginning";
         htmlReportData.StartText.Returns(startText);

         htmlReportBase.Start(configurationFilename);

         Received.InOrder(() => {
            file.Create(reportName);
            file.Write(startText);
         });
      }

      [TestMethod]
      public void WriteObservations_NothingAddedOrFailed_NothingIsWritten() {
         htmlReportBase.WriteObservations("testing", 0, 0, new TimeSpan(), new List<Observation> { ObservationStub });

         file.DidNotReceive().Write(Arg.Any<string>());
      }

      [TestMethod]
      public void WriteObservations_WritesResults() {
         const string header = "This is my stupid title";

         var observations = new List<Observation> { ObservationStub };
         htmlReportBase.WriteObservations(header, 1, 0, new TimeSpan(9, 7, 5, 3, 1), observations);

         Received.InOrder(() => {
            file.Write("<p>");
            file.Write("<div><div style=\"font-size: large; font-weight: bold; display: inline;\">" + header + ": 1</div> in 9.07:05:03.0010000</div>");
            htmlReportBase.WriteObservationsDetails(observations);
            file.Write("</p>");
         });
      }

      [TestMethod]
      public void WriteObservations_WritesAbsoluteValueNumberOfChanges() {
         htmlReportBase.WriteObservations(string.Empty, -2, 0, new TimeSpan(9, 7, 5, 3, 1), new List<Observation>());

         file.Write(Arg.Is<string>(s => s.Contains(": 2</h3>")));
      }

      [TestMethod]
      public void WriteObservations_NoThingFailed_DetailsAreNotWritten() {
         htmlReportBase.WriteObservations("testing", 1, 0, new TimeSpan(), new List<Observation>());

         htmlReportBase.DidNotReceive().WriteObservationsDetails(Arg.Any<List<Observation>>());
      }

      [TestMethod]
      public void Complete() {
         const string endText = "This is the end, my friend";
         htmlReportData.EndText.Returns(endText);

         htmlReportBase.Complete();

         Received.InOrder(() => {
            file.Write(endText);
            file.Received().Close();
         });
      }
   }
}
