using System;
using System.IO;

namespace SoftwareThresher.Utilities {

   public interface ISystemFileReader {
      void Open(string filename);
      string ReadLine();
      void Close();
   }

   class SystemFileReader : ISystemFileReader {
      StreamReader streamReader;

      public void Open(string filename) {
         if (streamReader != null) {
            throw new InvalidOperationException("Opening a document when the last one is still open.");
         }

         streamReader = new StreamReader(filename);
      }

      public string ReadLine() {
         return streamReader.ReadLine();
      }

      public void Close() {
         streamReader.Close();
         streamReader = null;
      }
   }
}
