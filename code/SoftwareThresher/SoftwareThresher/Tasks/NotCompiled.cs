using System.Collections.Generic;
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
         // TODO - Add option to search via Opengrok - via an enum for auto discovery and a factory
         // TODO - Combine path with the csproj and the path in the return to make sure they are the same
         observations.ForEach(o => o.Failed = true);

         var compileConfigurationFiles = search.GetFiles(Directory, CompileConfigurationFileSearchPattern);

         foreach (var file in compileConfigurationFiles) {
            var references = search.GetReferencesInFile(file, TextSearchPattern);


         }

         return observations;
      }
   }
}
