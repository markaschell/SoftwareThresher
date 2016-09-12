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
      }
   }
}
