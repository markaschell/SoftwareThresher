using System.Linq;

namespace SoftwareThresher.Settings.Search {
   public class OpenGrokParameter {
      readonly string label;
      readonly string value;

      public OpenGrokParameter(string label, string value) {
         this.label = label;
         this.value = value;
      }

      public override string ToString() {
         const string quotesString = "\"";

         var escapedValue = value.Replace(quotesString, $"\\{quotesString}");
         var quotes = escapedValue.Any(char.IsWhiteSpace) ? quotesString : string.Empty;

         return $"{label}={quotes}{escapedValue}{quotes}";
      }
   }
}
