using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Tasks {
   // TODO - change this to not on Disk?
   public class FindFilesOnDisk : Task, NoDetailsInReport {
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      Search systemDirectory;

      public FindFilesOnDisk() : this(new FileSystemSearch()) { }

      public FindFilesOnDisk(Search systemDirectory) {
         this.systemDirectory = systemDirectory;
      }

      public string ReportTitle { get { return "Found Files on Disk"; } }

      public List<Observation> Execute(List<Observation> observations) {
         var foundItems = systemDirectory.GetFiles(Directory, SearchPattern).ConvertAll(f => (Observation)new FileObservation(f));

         observations.AddRange(foundItems);

         return observations;
      }
   }
}
