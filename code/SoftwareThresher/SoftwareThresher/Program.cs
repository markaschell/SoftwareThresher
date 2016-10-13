using System.Linq;

namespace SoftwareThresher {
   class Program {
      static void Main(string[] args) {
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
