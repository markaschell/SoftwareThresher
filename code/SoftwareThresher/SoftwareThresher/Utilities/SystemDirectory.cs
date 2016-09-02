using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoftwareThresher.Utilities
{
    public interface ISystemDirectory
    {
        IEnumerable<string> GetFiles(string directory, string searchPattern);
    }

    public class SystemDirectory
    {
        public IEnumerable<string> GetFiles(string directory, string searchPattern)
        {
            return Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
        }
    }
}
