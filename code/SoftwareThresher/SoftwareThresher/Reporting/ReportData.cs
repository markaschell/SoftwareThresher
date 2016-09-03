using System;

namespace SoftwareThresher.Reporting {
   public interface IReportData {
      string GetFileNameWithoutExtesion(string configurationFilename);
      string GetTimestamp();

   }

   public class ReportData : IReportData {
      public string GetFileNameWithoutExtesion(string configurationFilename) {
         return configurationFilename.Split('.')[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
      }

      public string GetTimestamp() {
         return DateTime.Now.ToString("G");
      }
   }
}
