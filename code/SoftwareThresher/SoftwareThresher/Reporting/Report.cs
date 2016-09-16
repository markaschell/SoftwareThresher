using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public interface IReport {
      void Start(string configurationFilename);
      void WriteFindResults(string title, int observations, TimeSpan runningTime);
      void WriteObservations(string title, List<Observation> failedObservations, int totalObservations, TimeSpan runningTime);
      void Complete();
   }

   public class Report : IReport {
      ISystemFileWriter file;
      IReportData reportData;

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

      public void WriteFindResults(string title, int observations, TimeSpan runningTime) {
         file.Write(string.Format("<h3>{0}</h3> ({1}) in {2}", title, observations, runningTime.ToString("c")));
         file.Write("");
      }

      public void WriteObservations(string title, List<Observation> failedObservations, int totalObservations, TimeSpan runningTime) {
         if (failedObservations.Count == 0) {
            return;
         }

         file.Write(string.Format("<h3>{0}</h3> ({1} of {2}) in {3}", title, failedObservations.Count, totalObservations, runningTime.ToString("c")));

         foreach (var observation in failedObservations) {
            file.Write(string.Format("{0} - {1}", observation.Name, observation.Location));
         }

         file.Write("");
      }

      public void Complete() {
         file.Write("</body></html>");
         file.Close();
      }
   }
}
