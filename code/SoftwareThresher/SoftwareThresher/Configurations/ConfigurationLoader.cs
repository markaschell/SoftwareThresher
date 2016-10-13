using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationLoader {
      IConfiguration Load(string filename);
   }

   public class ConfigurationLoader : IConfigurationLoader {
      const BindingFlags FindBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
      const string SettingsSectionName = "settings";
      const string TasksSectionName = "tasks";
      const string SettingsNamespace = "SoftwareThresher.Settings.";
      const string TasksNamespace = "SoftwareThresher.Tasks.";

      readonly IConfigurationReader taskReader;

      public ConfigurationLoader(IConfigurationReader taskReader) {
         this.taskReader = taskReader;
      }

      public ConfigurationLoader() : this(new ConfigurationReader()) {
      }

      public IConfiguration Load(string filename) {
         try {
            taskReader.Open(filename);
            return LoadConfiguration();
         }
         finally {
            taskReader.Close();
         }
      }

      private Configuration LoadConfiguration() {
         var configuration = new Configuration();

         var settings = LoadSettings();

         foreach (var xmlTask in taskReader.GetNodes(TasksSectionName)) {
            var task = CreateTask(xmlTask.Name, settings);
            SetAttributes(xmlTask, task);
            configuration.Tasks.Add(task);
         }

         return configuration;
      }

      private List<object> LoadSettings() {
         var settings = new List<object>();

         foreach (var xmlSetting in taskReader.GetNodes(SettingsSectionName)) {
            var setting = CreateSetting(xmlSetting.Name);
            SetAttributes(xmlSetting, setting);
            settings.Add(setting);
         }

         return settings;
      }

      private static object CreateSetting(string settingName) {
         try {
            return Activator.CreateInstance(null, SettingsNamespace + settingName).Unwrap();
         }
         catch (Exception) {
            throw new NotSupportedException($"{settingName} is not a supported setting.");
         }
      }

      private static void SetAttributes(XmlNode input, object output) {
         foreach (var attribute in input.Attributes) {
            SetAttribute(attribute, output, input.Name);
         }
      }

      private static void SetAttribute(XmlAttribute attribute, object output, string xmlNodeName) {
         try {
            var property = output.GetType().GetProperty(attribute.Name, FindBindingFlags);
            property.SetValue(output, attribute.Value);
         }
         catch (Exception) {
            throw new NotSupportedException($"{attribute.Name} is not a supported attribute for {xmlNodeName}.");
         }
      }

      private static Task CreateTask(string taskName, List<object> settings) {
         var constructor = GetTaskConstuctor(taskName);
         var parameterValues = GetTaskParameterValues(constructor, settings, taskName);

         return (Task)constructor.Invoke(parameterValues.ToArray());
      }

      private static ConstructorInfo GetTaskConstuctor(string taskName) {
         var type = Type.GetType(TasksNamespace + taskName);

         if (type == null) {
            throw new NotSupportedException($"{taskName} is not a supported task.");
         }

         return type.GetConstructors(FindBindingFlags).Single();
      }

      private static List<object> GetTaskParameterValues(ConstructorInfo constructor, List<object> settings, string taskName) {
         var values = new List<object>();
         foreach (var parameter in constructor.GetParameters()) {
            var parameterValues = GetTaskParameterValue(settings, parameter.ParameterType);

            if (parameterValues.Count > 1) {
               throw new ArgumentNullException(parameter.ParameterType.Name, $"Multiple matching settings found for task {taskName}.");
            }

            if (!parameterValues.Any()) {
               throw new ArgumentNullException(parameter.ParameterType.Name, $"The constructor for task {taskName} has no defined setting.");
            }

            values.Add(parameterValues.First());
         }

         return values;
      }

      private static List<object> GetTaskParameterValue(IEnumerable<object> settings, Type parameterType) {
         return settings.Where(parameterType.IsInstanceOfType).ToList();
      }
   }
}
