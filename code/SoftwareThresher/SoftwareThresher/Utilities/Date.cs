using System;

namespace SoftwareThresher.Utilities {
   public class Date
   {
      readonly DateTime date;

      public static Date NullDate => new Date(DateTime.MinValue); 

      public Date(DateTime dateTime)
      {
         date = dateTime.Date;
      }

      public double DaysOld => Math.Floor((DateTime.Today - date).TotalDays);

      // TODO - ONLINE implement Equals

      public override string ToString() {
         return $"{date:MM/dd/yyyy}";
      }
   }
}
