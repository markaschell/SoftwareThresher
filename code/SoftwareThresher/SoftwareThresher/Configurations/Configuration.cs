using System.Collections.Generic;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations
{
    public interface IConfiguration
    {
        List<Task> Tasks { get; }
    }

    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            Tasks = new List<Task>();
        }

        public List<Task> Tasks { get; set; }
    }
}
