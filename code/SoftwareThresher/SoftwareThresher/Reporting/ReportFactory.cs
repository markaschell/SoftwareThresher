using System;
namespace SoftwareThresher.Reporting
{
    public interface IReportFactory
    {
        Report Create();
    }

    public class ReportFactory : IReportFactory
    {
        public Report Create()
        {
            throw new NotImplementedException();
        }
    }
}
