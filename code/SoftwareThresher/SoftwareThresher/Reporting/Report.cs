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

      const string HeaderStart = "<h3 style=\"display: inline;\">";
      const string HeaderEnd = "</h3>";
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

      public void WriteFindResults(string title, int observations, TimeSpan runningTime) {
         file.Write(string.Format("{0}{1} ({2}){3} in {4}{5}{5}", HeaderStart, title, observations, HeaderEnd, runningTime.ToString("c"), NewLine));
         file.Write("");
      }

      public void WriteObservations(string title, List<Observation> failedObservations, int totalObservations, TimeSpan runningTime) {
         if (failedObservations.Count == 0) {
            return;
         }

         file.Write(string.Format("{0}{1} ({2} of {3}){4} in {5}{6}", HeaderStart, title, failedObservations.Count, totalObservations, HeaderEnd, runningTime.ToString("c"), NewLine));

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
