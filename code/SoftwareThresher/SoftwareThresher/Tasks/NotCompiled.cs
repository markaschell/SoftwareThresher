using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   [UsageNote("Ignores case when matching names")]
   public class NotCompiled : Task {

      public string Directory { get; set; }

      public string CompileConfigurationFileSearchPattern { get; set; }

      public string StartTextSearchPattern { get; set; }

      [UsageNote("Must be on the same line as the StartTextSearchPattern")]
      public string EndTextSearchPattern { get; set; }

      public override string DefaultReportHeaderText => "Not Compiled";

      readonly Search search;

      public NotCompiled(Search search) {
         this.search = search;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         // TODO - does this stay here if we do Ors?  I think so.
         observations.ForEach(o => o.Failed = true);

         var observationsLookup = observations.ToLookup(o => o.Location);
         search.GetObservations(Directory, CompileConfigurationFileSearchPattern).ForEach(o => MarkObservationsPassedForFile(observationsLookup, o));

         return observations;
      }

      void MarkObservationsPassedForFile(ILookup<string, Observation> observations, Observation observation) {
         var observationsWithSameDirectory = observations.Where(o => o.Key.StartsWith(observation.Location)).SelectMany(g => g).ToLookup(o => o.Location);

         var referenceLines = search.GetReferenceLine(observation, StartTextSearchPattern);
         referenceLines.ForEach(r => MarkObservationsPassedForReference(observationsWithSameDirectory, observation.Location, r));
      }

      void MarkObservationsPassedForReference(ILookup<string, Observation> observations, string fileDirectory, string reference) {
         var referenceObject = GetReferenceObject(reference);

         if (string.IsNullOrEmpty(referenceObject))
            return;

         var referenceObservation = new FileObservation(referenceObject, null);
         var directory = Path.Combine(fileDirectory, referenceObservation.Location);

         observations.Where(o => string.Equals(o.Key, directory, StringComparison.CurrentCultureIgnoreCase))
                     .SelectMany(g => g).Where(o => string.Equals(referenceObservation.Name, o.Name, StringComparison.CurrentCultureIgnoreCase)).ToList()
                     .ForEach(o => o.Failed = false);
      }

      string GetReferenceObject(string reference)
      {
         var startIndex = reference.IndexOf(StartTextSearchPattern) + StartTextSearchPattern.Length;
         var endIndex = reference.IndexOf(EndTextSearchPattern, startIndex);
         var length = endIndex < startIndex ? 0 : endIndex - startIndex;

         return reference.Substring(startIndex, length).Trim();
      }
   }
}
