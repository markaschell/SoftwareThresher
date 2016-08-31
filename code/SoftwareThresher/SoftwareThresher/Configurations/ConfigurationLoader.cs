using System;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations
{
    public interface IConfigurationLoader
    {
        IConfiguration Load(string filename);
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

        public IConfiguration Load(string filename)
        {
            var configuration = new Configuration();

            xmlDocumentReader.Open(filename);

            var xmlTask = xmlDocumentReader.GetNextTask();
            while (xmlTask != null)
            {

                try
                {
                    configuration.Tasks.Add((Task)Activator.CreateInstance(null, "SoftwareThresher.Tasks." + xmlTask.Name).Unwrap());
                }
                catch (Exception e)
                {
                    throw new NotSupportedException(string.Format("{0} is not a supported task type.", xmlTask.Name));
                }

                xmlTask = xmlDocumentReader.GetNextTask();
            }

            xmlDocumentReader.Close();

            return configuration;
        }
    }
}
