﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Reporting {
   [TestClass]
   public class ReportTests {
      ISystemFileWriter file;
      IReportData reportData;

      Report report;

      [TestInitialize]
      public void Setup() {
         file = Substitute.For<ISystemFileWriter>();
         reportData = Substitute.For<IReportData>();

         report = new Report(file, reportData);
      }

      [TestMethod]
      public void Start() {
         var configurationFilename = "This is it";

         var reportName = "reportName";
         reportData.GetFileNameWithoutExtesion(configurationFilename).Returns(reportName);

         report.Start(configurationFilename);

         Received.InOrder(() => {
            file.Create(reportName + ".html");
            file.Write("<html><head></head><body>");
         });
      }

      [TestMethod]
      public void WriteObservations_WritesHeader() {
         var header = "This is my stupid title";

         report.WriteObservations(header, 1, 3, new TimeSpan(9, 7, 5, 3, 1), new List<Observation>());

         Received.InOrder(() => {
            file.Write("<h3 style=\"display: inline;\">" + header + ": 1 = 3</h3> in 9.07:05:03.0010000<br />");
            file.Write("<br />");
         });
      }

      [TestMethod]
      public void WriteObservations_ObservationData() {
         var name = "Issue";
         var location = "It is here";
         var observation = Substitute.For<Observation>();
         observation.Name.Returns(name);
         observation.Location.Returns(location);

         report.WriteObservations("", 1, 0, new TimeSpan(), new List<Observation> { observation });


         Received.InOrder(() => {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th></tr>");
            file.Write("<tr><td>" + name + "</td><td>" + location + "</td></tr>");
            file.Write("</table>");
         });
      }

      [TestMethod]
      public void WriteObservations_MultipleObservations() {
         var observation = Substitute.For<Observation>();

         report.WriteObservations("", 1, 0, new TimeSpan(), new List<Observation> { observation, observation });

         Received.InOrder(() => {
            file.Write(Arg.Is<string>(s => s.StartsWith("<h3")));
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write(Arg.Is<string>(s => s.Contains("th")));
            file.Write(Arg.Any<string>());
            file.Write(Arg.Any<string>());
            file.Write(Arg.Is<string>(s => s.Contains("table")));
            file.Write("<br />");
         });
      }

      [TestMethod]
      public void WriteObservations_NothingAddedOrFailed_NothingIsWritten() {
         var observation = Substitute.For<Observation>();

         report.WriteObservations("testing", 0, 3, new TimeSpan(), new List<Observation> { observation });

         file.DidNotReceive().Write(Arg.Any<string>());
      }

      [TestMethod]
      public void WriteObservations_NoThingFailed_TableIsNotWritten() {
         report.WriteObservations("testing", 1, 3, new TimeSpan(), new List<Observation>());

         file.DidNotReceive().Write(Arg.Is<string>(s => s.Contains("table")));
      }

      [TestMethod]
      public void Complete() {

         report.Complete();

         Received.InOrder(() => {
            file.Write("</body></html>");
            file.Received().Close();
         });
      }
   }
}
