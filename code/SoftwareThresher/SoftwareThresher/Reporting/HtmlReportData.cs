namespace SoftwareThresher.Reporting {
   public interface IHtmlReportData {
      string GetFileName(string configurationFilename);
      string StartText { get; }
      string EndText { get; }
      string NewLine { get; }
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
   }
}
