using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Searches;

namespace SoftwareThresher.Tasks {
   public class NotCompiled : Task {

      public string Directory { get; set; }

      public string CompileConfigurationFileSearchPattern { get; set; }

      // NOTE - RegEx format for FileSystemSearch
      public string TextSearchPattern { get; set; }

      public string ReportTitle { get { return "Not Compiled"; } }

      Search search;

      public NotCompiled() : this(new FileSystemSearch()) {
      }

      public NotCompiled(Search search) {
         this.search = search;
      }

      public List<Observation> Execute(List<Observation> observations) {
         observations.ForEach(o => o.Failed = true);

         var observationsLookup = observations.ToLookup(o => o.Location);
         // TODO - Last Performance - multi-threaded
         search.GetFiles(Directory, CompileConfigurationFileSearchPattern).ForEach(file => MarkObservationsPassedForFile(observationsLookup, file));

         return observations;
      }

      void MarkObservationsPassedForFile(ILookup<string, Observation> observations, string file) {
         var fileDirectory = new FileObservation(file).Location;
         // TODO - 2 Performance - use a hashmap or something similar
         // TODO - this performance does not support the use of relative paths in the found items....Should I need to handle this situation or can I get performance other ways?  Or should that be flagged because that is a bad practice.  Should this be reported via some other means?
         var observationsWithSameDirectory = observations.Where(o => o.Key.StartsWith(fileDirectory)).SelectMany(g => g);

         search.GetReferencesInFile(file, TextSearchPattern).ForEach(r => MarkObservationsPassedForReference(observationsWithSameDirectory, fileDirectory, r));
      }

      void MarkObservationsPassedForReference(IEnumerable<Observation> observations, string fileDirectory, string reference) {
         var referenceObject = reference.Substring(reference.IndexOf(TextSearchPattern) + TextSearchPattern.Length).Split(' ', '\t', '"').First();

         if (string.IsNullOrEmpty(referenceObject))
            return;

         var referenceObservation = new FileObservation(referenceObject);
         var directory = Path.Combine(fileDirectory, referenceObservation.Location);

         observations.Where(o => o.Location == directory && referenceObservation.Name == o.Name).ToList().ForEach(o => o.Failed = false);
      }
   }
}
