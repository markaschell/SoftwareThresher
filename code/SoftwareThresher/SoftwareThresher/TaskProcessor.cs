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
        IReportFactory reportFactory;

        public TaskProcessor(IConfigurationLoader configurationLoader, IReportFactory reportFactory)
        {
            this.configurationLoader = configurationLoader;
            this.reportFactory = reportFactory;
        }

        public TaskProcessor() : this(new ConfigurationLoader(), new ReportFactory())
        {
        }

        public void Run()
        {
            var configuration = configurationLoader.Load();
            var report = reportFactory.Create();

            var observations = new List<Observation>();
            foreach(var task in configuration.Tasks)
            {
                observations = task.Execute(observations.Where(o => o.Passed).ToList());

                report.WriteResults(task.ReportTitleForErrors, observations.Where(o => !o.Passed).ToList());
            }
        }
    }
}
