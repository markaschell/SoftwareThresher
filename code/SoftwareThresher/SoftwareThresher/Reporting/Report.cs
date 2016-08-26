using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Reporting
{
    public interface IReport
    {
        void Start(string configurationFilename);
        void WriteResults(string title, List<Observation> failedObservations);
        void Finialize();
    }

    public class Report : IReport
    {
        public void Start(string configurationFilename)
        {
            throw new NotImplementedException();
        }

        public void WriteResults(string title, List<Observation> failedObservations)
        {
            throw new NotImplementedException();
        }

        public void Finialize()
        {
            throw new NotImplementedException();
        }
    }
}
