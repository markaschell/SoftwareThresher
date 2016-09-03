using System;
using System.Reflection;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationLoader {
      IConfiguration Load(string filename);
   }

   public class ConfigurationLoader : IConfigurationLoader {
      ITaskReader taskReader;

      public ConfigurationLoader(ITaskReader taskReader) {
         this.taskReader = taskReader;
      }

      public ConfigurationLoader() : this(new TaskReader()) {
      }

      public IConfiguration Load(string filename) {
         taskReader.Open(filename);
         var configuration = LoadConfiguration();
         taskReader.Close();

         return configuration;
      }

      private Configuration LoadConfiguration() {
         var configuration = new Configuration();

         var xmlTask = taskReader.GetNextTask();
         while (xmlTask != null) {
            var task = CreateTask(xmlTask);
            SetAttributes(xmlTask, task);
            configuration.Tasks.Add(task);

            xmlTask = taskReader.GetNextTask();
         }

         return configuration;
      }

      private static Task CreateTask(XmlTask xmlTask) {
         try {
            return (Task)Activator.CreateInstance(null, "SoftwareThresher.Tasks." + xmlTask.Name).Unwrap();
         }
         catch (Exception) {
            throw new NotSupportedException(string.Format("{0} is not a supported task type.", xmlTask.Name));
         }
      }

      private static void SetAttributes(XmlTask xmlTask, Task task) {
         foreach (var attributeName in xmlTask.Attributes.Keys) {
            SetAttribute(xmlTask, task, attributeName);
         }
      }

      private static void SetAttribute(XmlTask xmlTask, Task task, string attributeName) {
         try {
            var property = task.GetType().GetProperty(attributeName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            property.SetValue(task, xmlTask.Attributes[attributeName]);
         }
         catch (Exception) {
            throw new NotSupportedException(string.Format("{0} is not a supported attribute for task type {1}.", attributeName, xmlTask.Name));
         }
      }
   }
}
