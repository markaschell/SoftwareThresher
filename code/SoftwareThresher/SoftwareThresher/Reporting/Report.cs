using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Reporting
{
    public interface IReport
    {
        void Start(string configurationFilename);
        void WriteResults(string title, List<Observation> failedObservations ,int totalObservations);
        void Complete();
    }

    public class Report : IReport
    {
        ISystemFile file;
        IReportData reportData;

        public Report() : this(new SystemFile(), new ReportData())
        { }

        public Report(ISystemFile file, IReportData reportData)
        {
            this.file = file;
            this.reportData = reportData;
        }

        public void Start(string configurationFilename)
        {
            var reportFileName = reportData.GetFileNameWithoutExtesion(configurationFilename) + ".html";
            file.Create(reportFileName);

            file.Write("<html><head></head><body>");
            file.Write(reportData.GetTimestamp());
        }

        public void WriteResults(string title, List<Observation> failedObservations, int totalObservations)
        {
            if (failedObservations.Count == 0)
            {
                return;
            }

            file.Write(string.Format("<h3>{0}</h3> - {1} of {2}", title, failedObservations.Count, totalObservations));

            foreach(var observation in failedObservations)
            {
                // TODO
                file.Write("");
            }
        }

        public void Complete()
        {
            file.Write(reportData.GetTimestamp());
            file.Write("</body></html>");
            file.Close();
        }
    }
}
