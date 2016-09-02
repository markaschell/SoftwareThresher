using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Tasks
{
    public class FindFiles : Task
    {
        public string Location { get; set; }
        public string SearchPattern { get; set; }

        ISystemDirectory systemDirectory;

        public FindFiles() : this(new SystemDirectory())
        { }

        public FindFiles(ISystemDirectory systemDirectory)
        {
            this.systemDirectory = systemDirectory;
        }

        public string ReportTitleForErrors
        {
            get { throw new NotImplementedException(); }
        }

        public List<Observation> Execute(List<Observation> observations)
        {
            return systemDirectory.GetFiles(Location, SearchPattern).ConvertAll(f => (Observation)new FileObservation(f));
        }
    }
}
