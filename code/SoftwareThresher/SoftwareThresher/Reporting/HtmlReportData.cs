using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public interface IHtmlReportData {
      string GetFileName(string configurationFilename);
      string StartText { get; }
      string EndText { get; }
      string NewLine { get; }
      string GetLastEditText(Observation observation);
   }

   public class HtmlReportData : IHtmlReportData {
      readonly IReportData reportData;

      public HtmlReportData() : this(new ReportData()) { }

      public HtmlReportData(IReportData reportData) {
         this.reportData = reportData;
      }

      public string GetFileName(string configurationFilename) {
         return reportData.GetFileNameWithoutExtesion(configurationFilename) + ".html";
      }

      public string StartText => "<html><head></head><body>";
      public string EndText => "</body></html>";
      public string NewLine => "<br />";

      public string GetLastEditText(Observation observation)
      {
         var lastEdit = observation.LastEdit;
         return lastEdit == Date.NullDate ? string.Empty : $"<a href='{observation.HistoryUrl}'>{lastEdit}</a>";
      }
   }
}
