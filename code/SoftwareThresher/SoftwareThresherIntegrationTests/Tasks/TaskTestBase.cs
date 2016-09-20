using System;
using SoftwareThresher;
using SoftwareThresher.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   public class TaskTestBase {

      protected TestReport report;

      TaskProcessor taskProcessor;

      [TestInitialize]
      public void Setup() {
         report = new TestReport();

         taskProcessor = new TaskProcessor(new ConfigurationLoader(), report);
      }

      protected void RunTest(string configurationFileName) {
         taskProcessor.Run("../../Tasks/" + configurationFileName);
      }

      public ReportItem LastReportItem { get { return report.LastReportItem; } }
   }
}
