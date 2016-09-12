using System;
using System.Xml;

namespace SoftwareThresher.Configurations {
   public interface ITaskReader {
      void Open(string filename);
      XmlTask GetNextTask();
      void Close();
   }

   public class TaskReader : ITaskReader {
      XmlReader xmlReader;

      public void Open(string filename) {
         if (xmlReader != null) {
            throw new InvalidOperationException("Opening an xml document while the last one is still open.");
         }

         xmlReader = XmlReader.Create(filename);

         xmlReader.ReadToFollowing("tasks");
         xmlReader.ReadStartElement();
      }

      public XmlTask GetNextTask() {
         if (!xmlReader.IsStartElement())
            return null;

         var xmlTask = new XmlTask { Name = xmlReader.Name };

         for (var i = 0; i < xmlReader.AttributeCount; i++) {
            xmlReader.MoveToAttribute(i);
            xmlTask.Attributes.Add(xmlReader.Name, xmlReader.Value);
         }

         xmlReader.Read();
         xmlReader.Read();

         return xmlTask;
      }

      public void Close() {
         xmlReader.Close();
         xmlReader = null;
      }
   }
}
