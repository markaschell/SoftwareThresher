﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Settings;
using SoftwareThresher.Tasks;

namespace SoftwareThresher.Configurations {
   public interface IConfigurationLoader {
      IConfiguration Load(string filename);
   }

   public class ConfigurationLoader : IConfigurationLoader {
      const string SettingsSectionName = "settings";
      const string TasksSectionName = "tasks";

      readonly IConfigurationReader taskReader;
      readonly IAttributeLoader attributeLoader;

      readonly IEnumerable<Type> taskTypes;
      readonly IEnumerable<Type> settingTypes;

      public ConfigurationLoader(IAssemblyObjectFinder assemblyObjectFinder, IConfigurationReader taskReader, IAttributeLoader attributeLoader) {
         this.taskReader = taskReader;
         this.attributeLoader = attributeLoader;

         taskTypes = assemblyObjectFinder.TaskTypes;
         settingTypes = assemblyObjectFinder.SettingTypes;
      }

      public ConfigurationLoader() : this(new AssemblyObjectFinder(), new ConfigurationReader(), new AttributeLoader()) {
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

      Configuration LoadConfiguration() {
         var configuration = new Configuration();

         var settings = LoadSettings();

         foreach (var xmlTask in taskReader.GetNodes(TasksSectionName)) {
            var task = CreateTask(xmlTask.Name, settings);
            task = (Task)attributeLoader.SetAttributes(xmlTask.Attributes, task);
            configuration.Tasks.Add(task);
         }

         return configuration;
      }

      List<Setting> LoadSettings() {
         var settings = new List<Setting>();

         foreach (var xmlSetting in taskReader.GetNodes(SettingsSectionName)) {
            var setting = CreateSetting(xmlSetting.Name);
            setting = (Setting)attributeLoader.SetAttributes(xmlSetting.Attributes, setting);
            settings.Add(setting);
         }

         return settings;
      }

      Setting CreateSetting(string settingName) {
         try {
            var settingType = settingTypes.Single(t => t.Name == settingName);

            return (Setting)Activator.CreateInstance(settingType);
         }
         catch (Exception) {
            throw new NotSupportedException($"{settingName} is not a supported setting.");
         }
      }

      Task CreateTask(string taskName, List<Setting> settings) {
         var constructor = GetTaskConstuctor(taskName);
         var parameterValues = GetTaskParameterValues(constructor, settings, taskName);

         return (Task)constructor.Invoke(parameterValues.ToArray());
      }

      ConstructorInfo GetTaskConstuctor(string taskName) {
         try {
            var taskType = taskTypes.Single(t => t.Name == taskName);

            return taskType.GetConstructors().Single();
         }
         catch (Exception) {
            throw new NotSupportedException($"{taskName} is not a supported task.");
         }
      }

      static List<object> GetTaskParameterValues(ConstructorInfo constructor, List<Setting> settings, string taskName) {
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

      static List<object> GetTaskParameterValue(IEnumerable<object> settings, Type parameterType) {
         return settings.Where(parameterType.IsInstanceOfType).ToList();
      }
   }
}
