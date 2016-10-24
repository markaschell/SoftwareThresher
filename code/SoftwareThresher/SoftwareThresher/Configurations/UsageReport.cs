using System;
using System.Linq;
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
            var parameters = task.GetConstructors().Single().GetParameters();
            var parameterText = string.Join(", ", parameters.Select(p => p.ParameterType.Name));
            console.WriteLine($"\tTask:\t{task.Name}({parameterText})");

            var properties = task.GetProperties().Where(a => a.CanWrite);
            foreach (var property in properties) {
               var noteAttribute = (UsageNoteAttribute)Attribute.GetCustomAttributes(property).FirstOrDefault(a => a.GetType() == typeof(UsageNoteAttribute));
               var noteText = noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;

               console.WriteLine($"\t\tAttribute:\t{property.Name} ({property.PropertyType.Name}){noteText}");
            }
         }
      }
   }
}
