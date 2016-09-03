using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting {
   public interface IReport {
      void Start(string configurationFilename);
      void WriteFindResults(string title, int observations);
      void WriteObservations(string title, List<Observation> failedObservations, int totalObservations);
      void Complete();
   }

   public class Report : IReport {
      ISystemFile file;
      IReportData reportData;

      public Report() : this(new SystemFile(), new ReportData()) { }

      public Report(ISystemFile file, IReportData reportData) {
         this.file = file;
         this.reportData = reportData;
      }

      public void Start(string configurationFilename) {
         var reportFileName = reportData.GetFileNameWithoutExtesion(configurationFilename) + ".html";
         file.Create(reportFileName);

         file.Write("<html><head></head><body>");
         file.Write(reportData.GetTimestamp());
         file.Write("");
      }

      const string HeaderNameFormat = "<h3>{0}</h3>";
      public void WriteFindResults(string title, int observations) {
         file.Write(string.Format(HeaderNameFormat + " ({1})", title, observations));
      }

      public void WriteObservations(string title, List<Observation> failedObservations, int totalObservations) {
         if (failedObservations.Count == 0) {
            return;
         }

         file.Write(string.Format(HeaderNameFormat + " ({1} of {2})", title, failedObservations.Count, totalObservations));

         foreach (var observation in failedObservations) {
            file.Write(string.Format("{0} - {1}", observation.Name, observation.Location));
         }

         file.Write("");
      }

      public void Complete() {
         file.Write(reportData.GetTimestamp());
         file.Write("</body></html>");
         file.Close();
      }
   }
}
