using System;
using System.IO;

namespace SoftwareThresher.Reporting {
   public interface IReportData {
      string GetFileNameWithoutExtesion(string configurationFilename);

   }

   public class ReportData : IReportData {
      public string GetFileNameWithoutExtesion(string configurationFilename) {
         return Path.GetFileNameWithoutExtension(configurationFilename) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
      }
   }
}
