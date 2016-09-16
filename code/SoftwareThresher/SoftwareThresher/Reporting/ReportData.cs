using System;

namespace SoftwareThresher.Reporting {
   public interface IReportData {
      string GetFileNameWithoutExtesion(string configurationFilename);
   }

   public class ReportData : IReportData {
      public string GetFileNameWithoutExtesion(string configurationFilename) {
         return configurationFilename.Split('.')[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
      }
   }
}
