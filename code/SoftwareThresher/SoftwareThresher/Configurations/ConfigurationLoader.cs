using System;

namespace SoftwareThresher.Configuration
{
    public interface IConfigurationLoader
    {
        Configuration Load(string filename);
    }

    public class ConfigurationLoader : IConfigurationLoader
    {
        public Configuration Load(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
