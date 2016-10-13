using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Settings {
   public interface Search {
      string BaseLocation { set; }

      List<Observation> GetObservations(string directory, string searchPattern);

      List<string> GetReferenceLine(Observation observation, string searchPattern);
   }
}