using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Settings.Search {
   public interface Search : Setting {
      string BaseLocation { set; }

      List<Observation> GetObservations(string location, string searchPattern);

      List<string> GetReferenceLine(Observation observation, string searchPattern);
   }
}