using System.Linq;
using SoftwareThresher.Configurations;

namespace SoftwareThresher {
   public class Program {
      public static void Main(string[] args) {
         if (!args.Any()) {
            new UsageReport().Write();
         }
         else {
            var taskProcessor = new TaskProcessor();
            args.ToList().ForEach(a => taskProcessor.Run(a));
         }
      }
   }
}
