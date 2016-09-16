using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Tasks;

namespace SoftwareThresher {
   public class TaskProcessor {
      IConfigurationLoader configurationLoader;
      IReport report;

      public TaskProcessor(IConfigurationLoader configurationLoader, IReport report) {
         this.configurationLoader = configurationLoader;
         this.report = report;
      }

      public TaskProcessor() : this(new ConfigurationLoader(), new Report()) {
      }

      public void Run(string configurationFilename) {
         var configuration = configurationLoader.Load(configurationFilename);
         report.Start(configurationFilename);

         try {
            var observations = new List<Observation>();
            foreach (var task in configuration.Tasks) {
               var orginalNumberOfObservations = observations.Count;

               var stopWatch = Stopwatch.StartNew();
               observations = task.Execute(observations.Where(o => !o.Failed).ToList());
               stopWatch.Stop();

               if (task is NoDetailsInReport) {
                  report.WriteFindResults(task.ReportTitle, observations.Count - orginalNumberOfObservations, stopWatch.Elapsed);
               }
               else {
                  report.WriteObservations(task.ReportTitle, observations.Where(o => o.Failed).ToList(), observations.Count, stopWatch.Elapsed);
               }
            }
         }
         finally {
            report.Complete();
         }
      }
   }
}
