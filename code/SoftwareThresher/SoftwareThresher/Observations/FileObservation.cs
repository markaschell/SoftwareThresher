using System.IO;

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
            get { return Path.GetDirectoryName(filename); }
        }

        public override string Name
        {
            get { return Path.GetFileName(filename); }
        }
    }
}
