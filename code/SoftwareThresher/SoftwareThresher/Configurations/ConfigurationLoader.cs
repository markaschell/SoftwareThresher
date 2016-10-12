using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationLoader {
      IConfiguration Load(string filename);
   }

   // TODO - split into seperate classes?  Test all together?
   public class ConfigurationLoader : IConfigurationLoader {
      const BindingFlags FindBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
      const string SettingsNamespace = "SoftwareThresher.Settings.";
      const string TasksNamespace = "SoftwareThresher.Tasks.";

      IConfigurationReader taskReader;

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

         foreach (var xmlTask in taskReader.GetTasks()) {
            var task = CreateTask(xmlTask.Name, settings);
            SetAttributes(xmlTask.Attributes, TasksNamespace, xmlTask.Name, task);
            configuration.Tasks.Add(task);
         }

         return configuration;
      }

      private List<object> LoadSettings() {
         var settings = new List<object>();

         foreach (var xmlSetting in taskReader.GetSettings()) {
            var setting = CreateSetting(xmlSetting.Type);
            SetAttributes(xmlSetting.Attributes, SettingsNamespace, xmlSetting.Name, setting);
            settings.Add(setting);
         }

         return settings;
      }

      private static object CreateSetting(string settingType) {
         try {
            return Activator.CreateInstance(null, SettingsNamespace + settingType).Unwrap();
         }
         catch (Exception) {
            throw new NotSupportedException(string.Format("{0} is not a supported setting type.", settingType));
         }
      }

      // TODO - object for the combination of the namespace and the type?
      private static void SetAttributes(List<XmlAttribute> attributeValues, string fromNamespace, string fromType, object toObject) {
         var typeToGetPropertiesFrom = GetType(fromNamespace, fromType);

         foreach (var attribute in attributeValues) {
            SetAttribute(attribute, typeToGetPropertiesFrom, toObject);
         }
      }

      private static Type GetType(string fromNamespace, string fromType) {
         var type = Type.GetType(fromNamespace + fromType);

         if (type == null) {
            // TODO - message that indicates the setting or task - pass down an enum?
            throw new NotSupportedException(string.Format("Unsupported type {0}.", fromType));
         }

         return type;
      }

      private static void SetAttribute(XmlAttribute attribute, Type typeToGetPropertiesFrom, object objectToSetAttributesOn) {
         try {
            var property = typeToGetPropertiesFrom.GetProperty(attribute.Name, FindBindingFlags);
            property.SetValue(objectToSetAttributesOn, attribute.Value);
         }
         catch (Exception) {
            // TODO - message that indicates the setting or task - pass down an enum?
            throw new NotSupportedException(string.Format("{0} is not a supported attribute for type {1}.", attribute.Name, typeToGetPropertiesFrom.Name));
         }
      }

      private static Task CreateTask(string taskName, List<object> settings) {
         var constructor = GetTaskConstuctor(taskName);
         var parameterValues = GetTaskParameterValues(constructor, settings, taskName);

         return (Task)constructor.Invoke(parameterValues.ToArray());
      }

      private static ConstructorInfo GetTaskConstuctor(string taskName) {
         var typeOfTask = GetType(TasksNamespace, taskName);
         return typeOfTask.GetConstructors(FindBindingFlags).Single();
      }

      private static List<object> GetTaskParameterValues(ConstructorInfo constructor, List<object> settings, string taskName) {
         var values = new List<object>();
         foreach (var parameter in constructor.GetParameters()) {
            var parameterValues = GetTaskParameterValue(settings, parameter.ParameterType);

            if (parameterValues.Count() > 1) {
               throw new ArgumentNullException(parameter.ParameterType.Name, string.Format("Multiple matching settings found for task {0}.", taskName));
            }
            else if (parameterValues.Count() == 0) {
               throw new ArgumentNullException(parameter.ParameterType.Name, string.Format("The constructor for task {0} has no defined setting.", taskName));
            }
            else {
               values.Add(parameterValues.First());
            }
         }

         return values;
      }

      private static IEnumerable<object> GetTaskParameterValue(List<object> settings, Type parameterType) {
         return settings.Where(o => parameterType.IsAssignableFrom(o.GetType()));
      }
   }
}
