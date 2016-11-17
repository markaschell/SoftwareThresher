using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public interface IReport {
      void Start(string configurationFilename);
      void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations);
      void Complete();
   }

   public class Report : IReport {
      readonly ISystemFileWriter file;
      readonly IReportData reportData;

      const string NewLine = "<br />";

      public Report() : this(new SystemFileWriter(), new ReportData()) { }

      public Report(ISystemFileWriter file, IReportData reportData) {
         this.file = file;
         this.reportData = reportData;
      }

      public void Start(string configurationFilename) {
         var reportFileName = reportData.GetFileNameWithoutExtesion(configurationFilename) + ".html";
         file.Create(reportFileName);

         file.Write("<html><head></head><body>");
      }

      public void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations) {
         if (changeInObservations == 0) {
            return;
         }

         file.Write($"<h3 style=\"display: inline;\">{title}: {changeInObservations} = {numberOfPassedObservations}</h3> in {runningTime:c}{NewLine}");

         if (failedObservations.Count > 0) {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write("<tr><th>Name</th><th>Location</th><th>Last Edited</th></tr>");
            foreach (var observation in failedObservations) {
               var lastEditString = observation.LastEdit == Date.NullDate ? string.Empty : $"<a href='{observation.HistoryUrl}'>{observation.LastEdit}</a>";

               file.Write($"<tr><td>{observation.Name}</td><td>{observation.Location}</td><td>{lastEditString}</td></tr>");
            }
            file.Write("</table>");
         }

         file.Write(NewLine);
      }

      public void Complete() {
         file.Write("</body></html>");
         file.Close();
      }
   }
}
