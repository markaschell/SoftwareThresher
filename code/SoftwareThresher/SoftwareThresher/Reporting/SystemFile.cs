using System.IO;

namespace SoftwareThresher.Reporting
{
    public interface ISystemFile
    {
        void Create(string filename);
        void Write(string text);
        void Close();
    }

    public class SystemFile : ISystemFile
    {
        StreamWriter streamWriter;

        public void Create(string filename)
        {
            streamWriter = new StreamWriter(filename);
            streamWriter.AutoFlush = true;
        }

        public void Write(string text)
        {
            streamWriter.WriteLineAsync(text);
        }
        public void Close()
        {
            streamWriter.Close();
            streamWriter.Dispose();
            streamWriter = null;
        }
    }
}
