using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Tasks {
   public class NotReferenced : Task {

      public string Location { get; set; }

      public string FileFilterPattern { get; set; }

      public int ExspectedNumberOfReferences { get; set; }

      public string ObservationNameRegExStringFormat { get; set; }

      // Assumes that all code files are referenced by a project file that is higher up the directory structure and not in a sibling folder stucture
      public string ReportTitle { get { return "Not Referenced"; } }

      Search search;

      public NotReferenced() : this(new OpenGrokSearch()) {
         FileFilterPattern = "*";
         ExspectedNumberOfReferences = 0;
         ObservationNameRegExStringFormat = "{0}";
      }

      public NotReferenced(Search search) {
         this.search = search;
      }

      // TODO - finish
      public List<Observation> Execute(List<Observation> observations) {
         throw new NotImplementedException(); 

         // Add option to search via Opengrok - via an enum for auto discovery and a factory
         // Combine path with the csproj and the path in the return to make sure they are the same


      

         //   foreach (var projectFile in projectFiles) {
         //      var codeFiles = nameExtractor.GetFiles(projectFile.Substring(0, projectFile.LastIndexOf('\\') + 1), FileExtensions.CSharp);
         //      var compileItems = ExtractNamesFromCspFile(projectFile);

         //      foreach (var codeFile in codeFiles) {
         //         if (!compileItems.Contains(GetFilenameFromPath(codeFile).ToLower())) {
         //            results.Add(codeFile);
         //         }
         //      }
         //   }

      }
   }
}
