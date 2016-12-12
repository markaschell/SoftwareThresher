using System;

namespace SoftwareThresher.Utilities {
   public class Date : IEquatable<Date> {
      readonly DateTime date;

      public static Date NullDate => new Date(DateTime.MinValue);

      public Date(DateTime dateTime) {
         date = dateTime.Date;
      }

      public double DaysOld => Math.Floor((DateTime.Today - date).TotalDays);

      public bool Equals(Date other) {
         if (other == null) {
            return false;
         }

         var otherDate = other.date;

         return date.Year == otherDate.Year && date.Month == otherDate.Month && date.Day == otherDate.Day;
      }

      public override bool Equals(object obj) {
         if (!(obj is Date)) {
            return false;
         }
         return Equals((Date)obj);
      }

      public override int GetHashCode() {
         return date.Year.GetHashCode() ^ date.Month.GetHashCode() ^ date.Day.GetHashCode();
      }

      public static bool operator ==(Date date1, Date date2) {
         if ((object)date1 == null || (object)date2 == null) {
            return Equals(date1, date2);
         }

         return date1.Equals(date2);
      }

      public static bool operator !=(Date date1, Date date2) {
         return !(date1 == date2);
      }

      public override string ToString() {
         return $"{date:MM/dd/yyyy}";
      }
   }
}
