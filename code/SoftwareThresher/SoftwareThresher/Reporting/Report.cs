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
      ISystemFileWriter file;
      IReportData reportData;

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

         file.Write(string.Format("<h3 style=\"display: inline;\">{0}: {1} = {2}</h3> in {3}{4}", title, changeInObservations, numberOfPassedObservations, runningTime.ToString("c"), NewLine));

         if (failedObservations.Count > 0) {
            file.Write("<table border=\"1\" style=\"border-collapse: collapse;\">");
            file.Write(string.Format("<tr><th>{0}</th><th>{1}</th></tr>", "Name", "Location"));
            foreach (var observation in failedObservations) {
               file.Write(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", observation.Name, observation.Location));
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
