using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Tasks {
   public class NotCompiled {

      public string Directory { get; set; }

      public string CompileConfigurationFileSearchPattern { get; set; }

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
            var references = search.GetReferencesInFile(file, TextSearchPattern);

            // TODO - Combine path with the csproj and the path in the return to make sure they are the same
            // TODO - Test that text in front of the search is ignored, text after the filename, if one filename is larger than the other, with path that matches or does not match
            var compiledConfigurationFileObservation = new FileObservation(file);

            foreach (var reference in references) {
               foreach (var observation in observations) {
                  var nameOfObjectReferenced = reference.Substring(reference.IndexOf(TextSearchPattern) + TextSearchPattern.Length, observation.Name.Length);
                  if (compiledConfigurationFileObservation.Location == observation.Location && nameOfObjectReferenced == observation.Name) {
                     observation.Failed = false;
                  }
               }
            }
         }

         return observations;
      }
   }
}
