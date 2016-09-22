namespace SoftwareThresher.Utilities {
   public interface IConsole {
      void WriteLine(string text);
   }

   public class Console : IConsole {
      public void WriteLine(string text) {
         System.Console.WriteLine(text);
      }
   }
}
