using System;
using System.Linq;
using SoftwareThresher.Configurations;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher {
   public class UsageReport {
      readonly IConsole console;
      readonly IClassFinder classFinder;

      public UsageReport() : this(new Console(), new ClassFinder()) { }

      public UsageReport(IConsole console, IClassFinder classFinder)
      {
         this.console = console;
         this.classFinder = classFinder;
      }

      public void Write() {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");

         foreach (var task in classFinder.TaskTypes) {
            console.WriteLine("\tTask:\t" + task.Name);

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
