using System;
using SoftwareThresher;
using SoftwareThresher.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareThresherIntegrationTests.Tasks {
   public class TaskTestBase {

      protected TestReport Report;

      TaskProcessor taskProcessor;

      [TestInitialize]
      public void Setup() {
         Report = new TestReport();

         taskProcessor = new TaskProcessor(new ConfigurationLoader(), Report);
      }

      protected void RunTest(string configurationFileName) {
         taskProcessor.Run("../../Tasks/" + configurationFileName);
      }

      public ReportItem LastReportItem => Report.LastReportItem;
   }
}
