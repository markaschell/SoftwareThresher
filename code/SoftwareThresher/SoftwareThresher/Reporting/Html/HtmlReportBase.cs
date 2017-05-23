using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Reporting.Html {
   public abstract class HtmlReportBase : Report {
      protected readonly ISystemFileWriter systemFileWriter;
      protected readonly IHtmlReportData htmlReportData;

      protected HtmlReportBase() : this(new SystemFileWriter(), new HtmlReportData()) { }

      protected HtmlReportBase(ISystemFileWriter systemFileWriter, IHtmlReportData htmlReportData) {
         this.systemFileWriter = systemFileWriter;
         this.htmlReportData = htmlReportData;
      }

      public void Start(string configurationFilename)
      {
         var reportFileName = htmlReportData.GetFileName(configurationFilename);
         systemFileWriter.Create(reportFileName);

         systemFileWriter.Write(htmlReportData.StartText);
      }

      public void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations)
      {
         if (changeInObservations == 0) {
            return;
         }

         systemFileWriter.Write("<p>");
         systemFileWriter.Write($@"<div><div style=""font-size: large; font-weight: bold; display: inline;"">{title}: {Math.Abs(changeInObservations)}</div> in {runningTime:c}</div>");

         if (failedObservations.Count > 0) {
            WriteObservationsDetails(failedObservations);
         }

         systemFileWriter.Write("</p>");
      }

      public abstract void WriteObservationsDetails(List<Observation> observations);

      public virtual void Complete()
      {
         systemFileWriter.Write(htmlReportData.EndText);
         systemFileWriter.Close();
      }
   }
}
