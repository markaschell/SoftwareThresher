using System.Text.RegularExpressions;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks.Filters {
   public class SearchPatternFilterParameter : FilterParameter {
      readonly string searchPattern;

      public SearchPatternFilterParameter(string searchPattern) {
         this.searchPattern = searchPattern;
      }

      public bool IsDefined => !string.IsNullOrEmpty(searchPattern);

      public bool ShouldFilter(Observation observation) {
         var regex = new Regex(searchPattern, RegexOptions.RightToLeft | RegexOptions.Singleline);
         return regex.IsMatch(observation.ToString());
      }
   }
}
