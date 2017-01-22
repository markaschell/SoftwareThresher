using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Reporting
{
   public interface Report {
      void Start(string configurationFilename);
      void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations);
      void Complete();
   }
}