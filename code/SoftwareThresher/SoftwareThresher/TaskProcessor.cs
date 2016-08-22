using System.Linq;
using SoftwareThresher.Configuration;

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

            configuration.Tasks.First().Execute();
        }
    }
}
