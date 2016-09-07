﻿using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Tasks {
   public class FindFilesOnDisk : Task, NoDetailsInReport {
      public string Directory { get; set; }

      public string SearchPattern { get; set; }

      ISystemDirectory systemDirectory;

      public FindFilesOnDisk() : this(new SystemDirectory()) { }

      public FindFilesOnDisk(ISystemDirectory systemDirectory) {
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
