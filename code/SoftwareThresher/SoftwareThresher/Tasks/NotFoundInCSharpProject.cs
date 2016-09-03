using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks {
      public class NotFoundInCSharpProject : Task {

      public string ReportTitle { get { return "Items not included in a C# project"; } }

      public List<Observation> Execute(List<Observation> observations) {
         throw new NotImplementedException();
      }
   }
}
