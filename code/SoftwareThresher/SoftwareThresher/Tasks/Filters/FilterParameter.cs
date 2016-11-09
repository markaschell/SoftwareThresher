using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks.Filters {
   public interface FilterParameter {
      bool IsDefined { get; }
      bool ShouldFilter(Observation observation);
   }
}
