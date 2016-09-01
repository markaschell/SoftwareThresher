using System.Collections.Generic;

namespace SoftwareThresher.Configurations
{
    public class XmlTask
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public XmlTask()
        {
            Attributes = new Dictionary<string, string>();
        }
    }
}
