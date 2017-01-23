using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public class HtmlTableReport : HtmlReportBase {
      public HtmlTableReport() { }

      public HtmlTableReport(ISystemFileWriter systemFileWriter, IHtmlReportData htmlReportData) : base(systemFileWriter, htmlReportData) { }

      public override void WriteObservationsDetails(List<Observation> observations) {
         systemFileWriter.Write(@"<table border=""1"" style=""border-collapse: collapse;"">");
         systemFileWriter.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");

         foreach (var observation in observations.OrderBy(o => o.SystemSpecificString)) {
            systemFileWriter.Write($"<tr><td>{observation.Name}</td><td>{observation.Location}</td><td>{htmlReportData.GetLastEditText(observation)}</td></tr>");
         }

         systemFileWriter.Write("</table>");
      }
   }
}