using System.Linq;

namespace SoftwareThresher {
   class Program {
      static void Main(string[] args) {
         if (args.Count() == 0) {

         }
         else {
            var taskProcessor = new TaskProcessor();
            args.ToList().ForEach(a => taskProcessor.Run(a));
         }
      }
   }
}
