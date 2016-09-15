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

         var compileConfigurationFiles = search.GetFiles(Directory, CompileConfigurationFileSearchPattern);

         foreach (var file in compileConfigurationFiles) {
            var baseDirectory = new FileObservation(file).Location;
            var references = search.GetReferencesInFile(file, TextSearchPattern);

            foreach (var reference in references) {
               var referenceObject = reference.Substring(reference.IndexOf(TextSearchPattern) + TextSearchPattern.Length).Split(' ', '\t', '"').First();

               if (string.IsNullOrEmpty(referenceObject))
                  continue;

               var referenceObservation = new FileObservation(referenceObject);

               foreach (var observation in observations) {
                  if (Path.Combine(baseDirectory, referenceObservation.Location) == observation.Location && referenceObservation.Name == observation.Name) {
                     observation.Failed = false;
                  }
               }
            }
         }

         return observations;
      }
   }
}
