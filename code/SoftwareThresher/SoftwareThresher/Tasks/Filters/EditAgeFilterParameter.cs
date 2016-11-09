﻿using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks.Filters {
   public class EditAgeFilterParameter : FilterParameter {
      readonly double editedInDays;

      public EditAgeFilterParameter(double editedInDays) {
         this.editedInDays = editedInDays;
      }

      public bool IsDefined => editedInDays > 0;

      public bool ShouldFilter(Observation observation) {
         return observation.LastEdit.DaysOld <= editedInDays;
      }
   }
}
