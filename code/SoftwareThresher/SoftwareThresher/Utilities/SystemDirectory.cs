using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoftwareThresher.Utilities
{
    public interface ISystemDirectory
    {
        List<string> GetFiles(string directory, string searchPattern);
    }

    public class SystemDirectory : ISystemDirectory
    {
        public List<string> GetFiles(string directory, string searchPattern)
        {
            return Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
        }
    }
}
