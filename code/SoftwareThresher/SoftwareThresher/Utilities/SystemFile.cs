using System;
using System.IO;

namespace SoftwareThresher.Utilities
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
            if (streamWriter != null)
            {
                throw new InvalidOperationException("Creating a document when the last one is still open.");
            }

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
            streamWriter = null;
        }
    }
}
