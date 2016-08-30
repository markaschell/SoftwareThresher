using System.Collections.Generic;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations
{
    public interface Configuration
    {
        List<Task> Tasks { get; }
    }
}
