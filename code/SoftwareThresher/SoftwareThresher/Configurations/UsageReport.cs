using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher.Configurations {
   public class UsageReport
   {
      const string Indent = "\t";

      readonly IConsole console;
      readonly IAssemblyObjectFinder assemblyObjectFinder;

      public UsageReport() : this(new AssemblyObjectFinder(), new Console()) { }

      public UsageReport(IAssemblyObjectFinder assemblyObjectFinder, IConsole console)
      {
         this.console = console;
         this.assemblyObjectFinder = assemblyObjectFinder;
      }

      public void Write()
      {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");
         
         WriteSettings();
         console.WriteLine("");
         WriteTasks();
      }

      void WriteSettings()
      {
         var settings = assemblyObjectFinder.SettingTypes.ToList();

         foreach (var settingInterface in assemblyObjectFinder.SettingInterfaces)
         {
            WriteSettingType(settingInterface, settings);
         }
      }

      void WriteSettingType(Type settingInterface, IEnumerable<Type> settings)
      {
         console.WriteLine($"{Indent}Setting Type:{Indent}{settingInterface.Name}");

         foreach (var setting in settings.Where(s => s.GetInterfaces().Contains(settingInterface)))
         {
            WriteSetting(setting);
         }
      }

      void WriteSetting(Type setting)
      {
         console.WriteLine($"{Indent}{Indent}Setting:{Indent}{setting.Name}");

         foreach (var property in GetProperties(setting))
         {
            WriteProperty(property, Indent);
         }
      }

      static IEnumerable<PropertyInfo> GetProperties(Type type) {
         return type.GetProperties().Where(a => a.CanWrite);
      }

      void WriteProperty(PropertyInfo property, string prefix = "") {
         var noteAttribute = (UsageNoteAttribute)Attribute.GetCustomAttributes(property).FirstOrDefault(a => a.GetType() == typeof(UsageNoteAttribute));
         var noteText = noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;

         console.WriteLine($"{prefix}{Indent}{Indent}Attribute:{Indent}{property.Name} ({property.PropertyType.Name}){noteText}");
      }

      void WriteTasks()
      {
         foreach (var task in assemblyObjectFinder.TaskTypes)
         {
            console.WriteLine($"{Indent}Task:{Indent}{task.Name}({GetParameterText(task)})");

            foreach (var property in GetProperties(task))
            {
               WriteProperty(property);
            }
         }
      }

      static string GetParameterText(Type task)
      {
         var parameters = task.GetConstructors().Single().GetParameters();
         return string.Join(", ", parameters.Select(p => p.ParameterType.Name));
      }
   }
}
