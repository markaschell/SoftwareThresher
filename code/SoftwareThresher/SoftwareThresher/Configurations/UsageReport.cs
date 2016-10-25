using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher.Configurations {
   public class UsageReport {
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

         foreach (var settingInterface in assemblyObjectFinder.SettingInterfaces) {
            console.WriteLine($"\tSetting Type:\t{settingInterface.Name}");

            foreach (var setting in settings.Where(s => s.GetInterfaces().Contains(settingInterface)))
            {
               console.WriteLine($"\t\tSetting:\t{setting.Name}");

               foreach (var property in GetProperties(setting)) {
                  WriteProperty(property, "\t");
               }
            }
         }
      }

      static IEnumerable<PropertyInfo> GetProperties(Type type) {
         return type.GetProperties().Where(a => a.CanWrite);
      }

      void WriteProperty(PropertyInfo property, string prefix = "") {
         var noteAttribute = (UsageNoteAttribute)Attribute.GetCustomAttributes(property).FirstOrDefault(a => a.GetType() == typeof(UsageNoteAttribute));
         var noteText = noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;

         console.WriteLine($"{prefix}\t\tAttribute:\t{property.Name} ({property.PropertyType.Name}){noteText}");
      }

      void WriteTasks()
      {
         foreach (var task in assemblyObjectFinder.TaskTypes)
         {
            console.WriteLine($"\tTask:\t{task.Name}({GetParameterText(task)})");

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
