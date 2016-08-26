using System.Linq;

namespace SoftwareThresher
{
    class Program
    {
        static void Main(string[] args)
        {
            new TaskProcessor().Run(args.First());
        }
    }
}
