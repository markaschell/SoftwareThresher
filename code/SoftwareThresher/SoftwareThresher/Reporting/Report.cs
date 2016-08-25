using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Reporting
{
    public interface Report
    {
        void WriteResults(string title, List<Observation> failedObservations);
    }
}
