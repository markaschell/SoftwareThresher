using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Settings.Search {
   public interface Search : Setting {
      string BaseLocation { set; }

      List<Observation> GetObservations(string location, string searchPattern);

      List<string> GetReferenceLine(Observation observation, string searchPattern);

      Date GetLastEditDate(Observation observation);

      string GetHistoryUrl(Observation observation);
   }
}