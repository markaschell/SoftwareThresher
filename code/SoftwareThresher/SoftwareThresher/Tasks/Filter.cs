using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks
{
    class Filter : Task, NoDetailsInReport
    {
        public string ReportTitle { get { return "Items Filtered"; } }

        public List<Observation> Execute(List<Observation> observations)
        {
            throw new NotImplementedException();
        }
    }
}
