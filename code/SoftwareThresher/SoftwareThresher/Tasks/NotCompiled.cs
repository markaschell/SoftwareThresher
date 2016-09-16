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

      // Assumes that all code files are referenced by a compile configuration file that is higher up the directory structure and not in a sibling folder stucture
      public string ReportTitle { get { return "Not Compiled"; } }

      Search search;

      public NotCompiled() : this(new FileSystemSearch()) {
      }

      public NotCompiled(Search search) {
         this.search = search;
      }

      public List<Observation> Execute(List<Observation> observations) {
         observations.ForEach(o => o.Failed = true);

         search.GetFiles(Directory, CompileConfigurationFileSearchPattern).ForEach(file => MarkObservationsPassedForFile(observations, file));

         return observations;
      }

      void MarkObservationsPassedForFile(IEnumerable<Observation> observations, string file) {
         var fileDirectory = new FileObservation(file).Location;
         var observationsWithSameDirectory = observations.Where(o => o.Location.Contains(fileDirectory));

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
