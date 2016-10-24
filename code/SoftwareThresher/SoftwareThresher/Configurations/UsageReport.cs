using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher.Configurations {
   public class UsageReport {
      readonly IConsole console;
      readonly IClassFinder classFinder;

      public UsageReport() : this(new ClassFinder(), new Console()) { }

      public UsageReport(IClassFinder classFinder, IConsole console)
      {
         this.console = console;
         this.classFinder = classFinder;
      }

      // TODO - Add settings 
      public void Write() {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");

         foreach (var task in classFinder.TaskTypes) {
            console.WriteLine($"\tTask:\t{task.Name}({GetParameterText(task)})");

            foreach (var property in GetProperties(task)) {
               WriteProperty(property);
            }
         }
      }

      static string GetParameterText(Type task)
      {
         var parameters = task.GetConstructors().Single().GetParameters();
         return string.Join(", ", parameters.Select(p => p.ParameterType.Name));
      }

      static IEnumerable<PropertyInfo> GetProperties(Type task) {
         return task.GetProperties().Where(a => a.CanWrite);
      }

      void WriteProperty(PropertyInfo property) {
         var noteAttribute = (UsageNoteAttribute) Attribute.GetCustomAttributes(property).FirstOrDefault(a => a.GetType() == typeof(UsageNoteAttribute));
         var noteText = noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;

         console.WriteLine($"\t\tAttribute:\t{property.Name} ({property.PropertyType.Name}){noteText}");
      }
   }
}
