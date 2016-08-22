using System;

namespace SoftwareThresher.Configuration
{
    public interface IConfigurationLoader
    {
        Configuration Load();
    }

    public class ConfigurationLoader : IConfigurationLoader
    {
        public Configuration Load()
        {
            throw new NotImplementedException();
        }
    }
}
