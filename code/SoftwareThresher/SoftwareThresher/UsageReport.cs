using System.Linq;
using SoftwareThresher.Tasks;
using SoftwareThresher.Utilities;

namespace SoftwareThresher {
   public class UsageReport {

      IConsole console;

      public UsageReport() : this(new Console()) { }

      public UsageReport(IConsole console) {
         this.console = console;
      }

      public void Write() {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");

         var tasks = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetInterfaces().Contains(typeof(Task)));

         foreach(var task in tasks) {
            console.WriteLine("\tTask:\t" + task.Name);

            var properties = task.GetProperties().Where(a => a.CanWrite);

            foreach(var property in properties) {
               var noteAttribute = (UsageNoteAttribute)System.Attribute.GetCustomAttributes(property).Where(a => a.GetType() == typeof(UsageNoteAttribute)).FirstOrDefault();
               var noteText = noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;

               console.WriteLine(string.Format("\t\tAttribute:\t{0} ({1}){2}", property.Name, property.PropertyType.Name, noteText));
            }
         }
      }
   }
}
