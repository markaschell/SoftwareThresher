using System;

namespace SoftwareThresher.Observations
{
    public class FileObservation : Observation
    {
        string filename;

        public FileObservation(string filename)
        {
            this.filename = filename;
        }

        public override string Location
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
                //Path.GetFileName
            }
        }
    }
}
