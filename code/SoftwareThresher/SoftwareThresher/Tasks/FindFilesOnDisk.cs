﻿using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Settings;

namespace SoftwareThresher.Tasks {
   public class FindFilesOnDisk : Task {
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      Search search;

      public FindFilesOnDisk(Search systemDirectory) {
         this.search = systemDirectory;
      }

      public string ReportTitle { get { return "Found Files on Disk"; } }

      public List<Observation> Execute(List<Observation> observations) {
         var foundItems = search.GetFiles(Directory, SearchPattern).ConvertAll(f => (Observation)new FileObservation(f));

         observations.AddRange(foundItems);

         return observations;
      }
   }
}
