using System;
using System.Collections.Generic;
using System.Linq;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public class HtmlTableReport : Report {
      readonly ISystemFileWriter file;
      readonly IHtmlReportData htmlReportData;

      public HtmlTableReport() : this(new SystemFileWriter(), new HtmlReportData()) { }

      public HtmlTableReport(ISystemFileWriter file, IHtmlReportData htmlReportData) {
         this.file = file;
         this.htmlReportData = htmlReportData;
      }

      public void Start(string configurationFilename) {
         var reportFileName = htmlReportData.GetFileName(configurationFilename);
         file.Create(reportFileName);

         file.Write(htmlReportData.StartText);
      }

      public void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations) {
         if (changeInObservations == 0) {
            return;
         }

         file.Write($"<h3 style=\"display: inline;\">{title}: {Math.Abs(changeInObservations)}</h3> in {runningTime:c}{htmlReportData.NewLine}");

         if (failedObservations.Count > 0) {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");
            foreach (var observation in failedObservations.OrderBy(o => o.SystemSpecificString)) {
               var lastEdit = observation.LastEdit;
               var lastEditString = lastEdit == Date.NullDate ? string.Empty : $"<a href='{observation.HistoryUrl}'>{lastEdit}</a>";

               file.Write($"<tr><td>{observation.Name}</td><td>{observation.Location}</td><td>{lastEditString}</td></tr>");
            }
            file.Write("</table>");
         }

         file.Write(htmlReportData.NewLine);
      }

      public void Complete() {
         file.Write(htmlReportData.EndText);
         file.Close();
      }
   }
}
