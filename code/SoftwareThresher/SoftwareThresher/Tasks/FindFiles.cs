using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks
{
    public class FindFiles : Task
    {
        public string Location { get; set; }
        public string SearchPattern { get; set; }

        public string ReportTitleForErrors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<Observation> Execute(List<Observation> observations)
        {
            throw new NotImplementedException();


        }
    }
}
