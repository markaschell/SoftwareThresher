using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;

namespace SoftwareThresherIntegrationTests.Tasks {
   public class TestReport : IReport {

      public ReportItem LastReportItem { get; private set; }

      public void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations) {
         LastReportItem = new ReportItem(changeInObservations, failedObservations);
      }

      public void Start(string configurationFilename) { }

      public void Complete() { }
   }

   public class ReportItem {
      public int ChangeInObservations { get; }
      public List<Observation> FailedObservations { get; }

      public ReportItem(int changeInObservations, List<Observation> failedObservations) {
         ChangeInObservations = changeInObservations;
         FailedObservations = failedObservations;
      }
   }
}
