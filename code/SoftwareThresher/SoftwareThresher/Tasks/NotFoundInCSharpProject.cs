using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks
{
    public class NotFoundInCSharpProject : Task
    {
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
