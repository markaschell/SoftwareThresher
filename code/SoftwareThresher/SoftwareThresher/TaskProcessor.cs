using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;

namespace SoftwareThresher {
   public class TaskProcessor {
      readonly IConfigurationLoader configurationLoader;
      readonly IReport report;

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

               var failedObservations = observations.Where(o => o.Failed).ToList();
               var numberOfPassedObservations = observations.Count - failedObservations.Count;
               report.WriteObservations(task.ReportHeader, numberOfPassedObservations - orginalNumberOfObservations, numberOfPassedObservations, stopWatch.Elapsed, failedObservations);
            }
         }
         finally {
            report.Complete();
         }
      }
   }
}
