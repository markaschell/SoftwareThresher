using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresher.Tasks {
   public class NotCompiled : Task {

      public string Directory { get; set; }

      public string CompileConfigurationFileSearchPattern { get; set; }

      [UsageNote("Format is RegEx")]
      public string TextSearchPattern { get; set; }

      public override string DefaultReportHeaderText => "Not Compiled";

      readonly Search search;

      public NotCompiled(Search search) {
         this.search = search;
      }

      public override List<Observation> Execute(List<Observation> observations) {
         observations.ForEach(o => o.Failed = true);

         var observationsLookup = observations.ToLookup(o => o.Location);
         search.GetObservations(Directory, CompileConfigurationFileSearchPattern).ForEach(o => MarkObservationsPassedForFile(observationsLookup, o));

         return observations;
      }

      void MarkObservationsPassedForFile(ILookup<string, Observation> observations, Observation observation) {
         var observationsWithSameDirectory = observations.Where(o => o.Key.StartsWith(observation.Location)).SelectMany(g => g).ToLookup(o => o.Location);

         search.GetReferenceLine(observation, TextSearchPattern).ForEach(r => MarkObservationsPassedForReference(observationsWithSameDirectory, observation.Location, r));
      }

      void MarkObservationsPassedForReference(ILookup<string, Observation> observations, string fileDirectory, string reference) {
         var referenceObject = reference.Substring(reference.IndexOf(TextSearchPattern) + TextSearchPattern.Length).Split(' ', '\t', '"').First();

         if (string.IsNullOrEmpty(referenceObject))
            return;

         var referenceObservation = new FileObservation(referenceObject, null);
         var directory = Path.Combine(fileDirectory, referenceObservation.Location);

         // TODO - The ToLowers are not 100% accurate but given the way that Visual Studio ignores case when reading the csproj file this is the current solution 
         // TODO - do we have a similar issue for directory?
         observations.Where(o => o.Key == directory).SelectMany(g => g).Where(o => referenceObservation.Name.ToLower() == o.Name.ToLower()).ToList().ForEach(o => o.Failed = false);
      }
   }
}
