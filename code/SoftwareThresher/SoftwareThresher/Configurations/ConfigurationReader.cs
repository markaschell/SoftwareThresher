using System;
using System.Collections.Generic;
using System.Xml;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationReader {
      void Open(string filename);
      List<XmlNode> GetNodes(string sectionName);
      void Close();
   }

   public class ConfigurationReader : IConfigurationReader {
      XmlReader xmlReader;

      public void Open(string filename) {
         if (xmlReader != null) {
            throw new InvalidOperationException("Opening an xml document while the last one is still open.");
         }

         xmlReader = XmlReader.Create(filename);
      }

      public List<XmlNode> GetNodes(string sectionName) {
         MoveToFirstChildNode(sectionName);

         var nodes = new List<XmlNode>();
         while (xmlReader.IsStartElement()) {
            var xmlNode = new XmlNode { Name = xmlReader.Name };

            for (var i = 0; i < xmlReader.AttributeCount; i++) {
               xmlReader.MoveToAttribute(i);
               xmlNode.Attributes.Add(new XmlAttribute { Name = xmlReader.Name, Value = xmlReader.Value });
            }

            MoveToNextNode();
         }

         return nodes;
      }

      private void MoveToFirstChildNode(string sectionName) {
         xmlReader.ReadToFollowing(sectionName);
         xmlReader.ReadStartElement();
      }

      private void MoveToNextNode() {
         xmlReader.Read();
         xmlReader.Read();
      }

      public void Close() {
         xmlReader.Close();
         xmlReader = null;
      }
   }
}
