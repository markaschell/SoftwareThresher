using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

// TODO - organize this clases better in the file system
namespace SoftwareThresher.Reporting {
   public abstract class HtmlReportBase : Report {
      protected readonly ISystemFileWriter file;
      protected readonly IHtmlReportData htmlReportData;

      protected HtmlReportBase() : this(new SystemFileWriter(), new HtmlReportData()) { }

      protected HtmlReportBase(ISystemFileWriter file, IHtmlReportData htmlReportData) {
         this.file = file;
         this.htmlReportData = htmlReportData;
      }

      public void Start(string configurationFilename)
      {
         var reportFileName = htmlReportData.GetFileName(configurationFilename);
         file.Create(reportFileName);

         file.Write(htmlReportData.StartText);
      }

      public abstract void WriteObservations(string title, int changeInObservations, int numberOfPassedObservations, TimeSpan runningTime, List<Observation> failedObservations);

      public virtual void Complete()
      {
         file.Write(htmlReportData.EndText);
         file.Close();
      }
   }
}
