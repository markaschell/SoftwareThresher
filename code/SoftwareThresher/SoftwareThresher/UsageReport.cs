using System;
using System.Linq;
using SoftwareThresher.Tasks;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher {
   public class UsageReport {
      readonly IConsole console;

      public UsageReport() : this(new Console()) { }

      public UsageReport(IConsole console) {
         this.console = console;
      }

      public void Write() {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");

         var tasks = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetInterfaces().Contains(typeof(Task)));

         foreach (var task in tasks) {
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
