using System;
using System.Text;
using System.Web;
// ReSharper disable InconsistentNaming

namespace SoftwareThresher.Settings.Search {
   public class OpenGrokJsonSearchResult {
      public string directory { get; set; }
      public string filename { get; set; }
      public string lineno { get; set; }

      string _line;
      public string line {
         get { return _line; }
         set {
            var readableString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            var removedFormatting = readableString.Replace("<b>", string.Empty).Replace("</b>", string.Empty);
            _line = HttpUtility.HtmlDecode(removedFormatting);
         }
      }
      //public string path { get; set; }
   }
}
