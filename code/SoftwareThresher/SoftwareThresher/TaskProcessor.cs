using System.Collections.Generic;
using SoftwareThresher.Configuration;
using SoftwareThresher.Observations;

namespace SoftwareThresher
{
    public class TaskProcessor
    {
        IConfigurationLoader configurationLoader;

        public TaskProcessor(IConfigurationLoader configurationLoader)
        {
            this.configurationLoader = configurationLoader;
        }

        public TaskProcessor() : this(new ConfigurationLoader())
        {
        }

        public void Run()
        {
            var configuration = configurationLoader.Load();

            var observations = new List<Observation>();
            foreach(var task in configuration.Tasks)
            {
                observations = task.Execute(observations);
            }
        }
    }
}
