using System.Collections.Generic;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configuration
{
    public interface Configuration
    {
        List<Task> Tasks { get; }
    }
}
