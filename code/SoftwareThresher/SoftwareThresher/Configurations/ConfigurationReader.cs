using System;
using System.Collections.Generic;
using System.Xml;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationReader {
      void Open(string filename);
      List<XmlSetting> GetSettings();
      List<XmlTask> GetTasks();
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

      public List<XmlSetting> GetSettings() {
         MoveToFirstChildNode("settings");

         var settings = new List<XmlSetting>();
         while (xmlReader.IsStartElement()) {
            var xmlSetting = new XmlSetting { Name = xmlReader.Name };

            for (var i = 0; i < xmlReader.AttributeCount; i++) {
               xmlReader.MoveToAttribute(i);

               if (xmlReader.Name == "Type") {
                  xmlSetting.Type = xmlReader.Value;
               }
               else {
                  xmlSetting.Attributes.Add(new XmlAttribute { Name = xmlReader.Name, Value = xmlReader.Value });
               }
            }

            MoveToNextNode();
         }

         return settings;
      }

      private void MoveToFirstChildNode(string sectionName) {
         xmlReader.ReadToFollowing(sectionName);
         xmlReader.ReadStartElement();
      }

      private void MoveToNextNode() {
         xmlReader.Read();
         xmlReader.Read();
      }

      public List<XmlTask> GetTasks() {
         MoveToFirstChildNode("tasks");

         var tasks = new List<XmlTask>();
         while (xmlReader.IsStartElement()) {
            var xmlTask = new XmlTask { Name = xmlReader.Name };

            for (var i = 0; i < xmlReader.AttributeCount; i++) {
               xmlReader.MoveToAttribute(i);
               xmlTask.Attributes.Add(new XmlAttribute { Name = xmlReader.Name, Value = xmlReader.Value });
            }

            MoveToNextNode();
         }

         return tasks;
      }

      public void Close() {
         xmlReader.Close();
         xmlReader = null;
      }
   }
}
