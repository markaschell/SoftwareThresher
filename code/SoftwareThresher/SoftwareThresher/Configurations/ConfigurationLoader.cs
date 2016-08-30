using System;

namespace SoftwareThresher.Configurations
{
    public interface IConfigurationLoader
    {
        Configuration Load(string filename);
    }

    public class ConfigurationLoader : IConfigurationLoader
    {
        IXmlDocumentReader xmlDocumentReader;

        public ConfigurationLoader(IXmlDocumentReader xmlDocumentReader)
        {
            this.xmlDocumentReader = xmlDocumentReader;
        }

        public ConfigurationLoader() : this(new XmlDocumentReader())
        {
        }

        public Configuration Load(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
