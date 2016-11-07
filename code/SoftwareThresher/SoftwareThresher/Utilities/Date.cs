using System;
namespace SoftwareThresher.Utilities {
   public class Date
   {
      readonly DateTime date;

      public Date(DateTime dateTime)
      {
         date = dateTime.Date;
      }

      public double DaysOld => Math.Floor((DateTime.Today - date).TotalDays);
   }
}
