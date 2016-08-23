using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks
{
    public interface Task
    {
        List<Observation> Execute(List<Observation> observations);
    }
}
