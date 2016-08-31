using System.Xml;

namespace SoftwareThresher.Configurations
{
    public interface IXmlDocumentReader
    {
        void Open(string filename);
        XmlTask GetNextTask();
        void Close();

    }

    public class XmlDocumentReader : IXmlDocumentReader
    {
        XmlReader xmlReader;

        public void Open(string filename)
        {
            xmlReader = XmlReader.Create(filename);

            xmlReader.ReadToFollowing("tasks");
            xmlReader.ReadStartElement();
        }

        public XmlTask GetNextTask()
        {
            if (!xmlReader.IsStartElement())
                return null;

            var xmlTask = new XmlTask { Name = xmlReader.Name };

            for (var i = 0; i < xmlReader.AttributeCount; i++)
            {
                xmlReader.MoveToAttribute(i);
                xmlTask.Attributes.Add(xmlReader.Name, xmlReader.Value);
            }

            xmlReader.MoveToElement();
            xmlReader.ReadStartElement();

            return xmlTask;
        }

        public void Close()
        {
            xmlReader.Close();
            xmlReader = null;
        }
    }
}
