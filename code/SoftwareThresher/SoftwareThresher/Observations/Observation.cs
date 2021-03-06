﻿using System.IO;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Observations {
   public abstract class Observation {
      readonly Search search;

      protected Observation(Search search)
      {
         this.search = search;
      }

      public virtual bool Failed { get; set; }

      public abstract string Name { get; }

      public abstract string Location { get; }

      public abstract string SystemSpecificString { get; }

      public virtual Date LastEdit => search.GetLastEditDate(this);

      public virtual string HistoryUrl => search.GetHistoryUrl(this);

      public override string ToString() {
         return $"{Location}{Path.PathSeparator}{Name}";
      }

      public string FilenameWithoutExtension => Path.GetFileNameWithoutExtension(Name);
   }
}
