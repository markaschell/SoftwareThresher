using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   // TODO - Do we want this to be a child or a strategy?
   // TODO - organize these clases better in the systemFileWriter system
   public class HtmlTreeReport : HtmlReportBase {
      public HtmlTreeReport() { }

      public HtmlTreeReport(ISystemFileWriter systemFileWriter, IHtmlReportData htmlReportData) : base(systemFileWriter, htmlReportData) { }

      // TODO - finish
      public override void WriteObservationsDetails(List<Observation> observations) {
         systemFileWriter.Write(@"<table id=""resultsTable"">");

         foreach (var observation in observations.OrderBy(o => o.SystemSpecificString)) {
            var lastEdit = observation.LastEdit;
            var lastEditString = lastEdit == Date.NullDate ? string.Empty : $"<a href='{observation.HistoryUrl}'>{lastEdit}</a>";

            systemFileWriter.Write($@"<tr data-depth=""0""><td>{observation.SystemSpecificString}</td><td>{lastEditString}</td></tr>");
         }
         systemFileWriter.Write("</table>");
      }

      public override void Complete() {
         // TODO write script
         systemFileWriter.Write("");
         base.Complete();
      }
   }
}
