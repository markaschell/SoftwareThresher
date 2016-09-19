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
         file.Write("");
      }

      public void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations) {
         if (changeInObservations == 0) {
            return;
         }

         file.Write(string.Format("<h3 style=\"display: inline;\">{0}: {1} = {2}</h3> in {3}{4}", title, changeInObservations, numberOfPassedObservations, runningTime.ToString("c"), NewLine));

         foreach (var observation in failedObservations) {
            file.Write(string.Format("<span style=\"white-space: nowrap;\">{0} - {1}</span>{2}", observation.Name, observation.Location, NewLine));
         }

         file.Write("");
      }

      public void Complete() {
         file.Write("</body></html>");
         file.Close();
      }
   }
}
