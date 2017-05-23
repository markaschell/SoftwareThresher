using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting.Html {
   // TODO - Do we want this to be a child or a strategy?
   // TODO - Do we need this or should be be creating a grouping one that groups per pre-defined ways or is that dynamic?
   public class HtmlTreeReport : HtmlReportBase {
      public HtmlTreeReport() { }

      public HtmlTreeReport(ISystemFileWriter systemFileWriter, IHtmlReportData htmlReportData) : base(systemFileWriter, htmlReportData) { }

      // TODO - finish
      public override void WriteObservationsDetails(List<Observation> observations) {
         systemFileWriter.Write(@"<table id=""resultsTable"">");

         foreach (var observation in observations.OrderBy(o => o.SystemSpecificString)) {
            systemFileWriter.Write($@"<tr data-depth=""0""><td>{observation.SystemSpecificString}</td><td>{htmlReportData.GetLastEditText(observation)}</td></tr>");
         }
         systemFileWriter.Write(@"</table>");
      }

      public override void Complete() {
         // TODO write script
         systemFileWriter.Write("");
         base.Complete();
      }
   }
}
