using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace SoftwareThresher.Settings.Search {
   // As defined here: https://github.com/OpenGrok/OpenGrok/wiki/OpenGrok-web-services
   public class OpenGrokJsonSearchResponse {
      //public string duration { get; set; }
      public string resultcount { get; set; }
      public string maxresults { get; set; }
      public List<OpenGrokJsonSearchResult> results { get; set; }
   }
}
