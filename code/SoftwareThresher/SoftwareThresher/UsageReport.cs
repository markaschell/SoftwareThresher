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

            var attributes = task.GetProperties().Where(a => a.CanWrite);

            foreach(var attribute in attributes) {
               console.WriteLine(string.Format("\t\tAttribute:\t{0} ({1})", attribute.Name, attribute.PropertyType.Name));
            }
         }
      }
   }
}
