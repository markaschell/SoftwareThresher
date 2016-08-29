using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Configuration;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;

namespace SoftwareThresher
{
    public class TaskProcessor
    {
        IConfigurationLoader configurationLoader;
        IReport report;

        public TaskProcessor(IConfigurationLoader configurationLoader, IReport report)
        {
            this.configurationLoader = configurationLoader;
            this.report = report;
        }

        public TaskProcessor() : this(new ConfigurationLoader(), new Report())
        {
        }

        public void Run(string configurationFilename)
        {
            var configuration = configurationLoader.Load(configurationFilename);
            report.Start(configurationFilename);

            try
            {
                var observations = new List<Observation>();
                foreach (var task in configuration.Tasks)
                {
                    observations = task.Execute(observations.Where(o => o.Passed).ToList());

                    report.WriteResults(task.ReportTitleForErrors, observations.Where(o => !o.Passed).ToList(), observations.Count);
                }
            }
            finally
            {
                report.Complete();
            }
        }
    }
}
