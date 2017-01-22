using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;

namespace SoftwareThresher {
   public class TaskProcessor {
      readonly IConfigurationLoader configurationLoader;
      readonly Report report;

      public TaskProcessor(IConfigurationLoader configurationLoader, Report report) {
         this.configurationLoader = configurationLoader;
         this.report = report;
      }

      // TODO - make the report configurable
      public TaskProcessor() : this(new ConfigurationLoader(), new HtmlTableReport()) {
      }

      public void Run(string configurationFilename) {
         var configuration = configurationLoader.Load(configurationFilename);
         report.Start(configurationFilename);

         try {
            var observations = new List<Observation>();
            foreach (var task in configuration.Tasks) {
               var orginalNumberOfObservations = observations.Count;

               var stopWatch = Stopwatch.StartNew();
               observations = task.Execute(observations);
               stopWatch.Stop();

               var failedObservations = observations.Where(o => o.Failed).ToList();
               var numberOfPassedObservations = observations.Count - failedObservations.Count;
               report.WriteObservations(task.ReportHeader, numberOfPassedObservations - orginalNumberOfObservations, numberOfPassedObservations, stopWatch.Elapsed, failedObservations);
               observations = observations.Where(o => !o.Failed).ToList();
            }
         }
         finally {
            report.Complete();
         }
      }
   }
}
