using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SoftwareThresher.Searches {

   public class OpenGrokJsonSearch : Search {

      const string TextSearchParameterLabel = "freetext=";
      //const string DefinitionSearchParameterLabel = "def=";
      //const string SymbolSearchParameterLabel = "symbol=";
      const string PathSearchParameterLabel = "path=";
      //const string HistorySearchParameterLabel = "hist=";
      const string ParameterJoin = "&";

      public List<string> GetFiles(string directory, string searchPattern) {
         // TODO - Add example?
         // TODO - Rename directory include the task parameter?
         // TODO - Add type as a parameter the the task or define it for all tasks?  Play with injection and how that will work with reflection....Assign parameters in Task or pass into the constructor?  Should we default all of the items in the base?
         // TODO - Could we use "file://" in the directory to search locally and just use the same Search?  Probably not because of the different return types.  Use that to access svn server - wonder which is faster

         return GetResults(TextSearchParameterLabel + searchPattern, (r) => r.path);
      }

      public List<string> GetReferencesInFile(string filename, string searchPattern) {
         // TODO - since this requires the whole path should we not pass it in to other tasks
         return GetResults(PathSearchParameterLabel + filename + ParameterJoin + TextSearchParameterLabel + searchPattern, (r) => r.line);
      }

      List<string> GetResults(string parameters, Converter<OpenGrokJsonSearchResult, string> converter) {
         // TODO - could opengrok be configured to a different location?  {host}/{webapp_name}/json?freetext
         var request = WebRequest.Create("http://opengrok/source/json?" + parameters);

         using (var response = request.GetResponse()) {
            var serializer = new DataContractJsonSerializer(typeof(OpenGrokJsonSearchResponse));
            var searchResponse = (OpenGrokJsonSearchResponse)serializer.ReadObject(response.GetResponseStream());

            return searchResponse.results.ConvertAll(converter);
         }
      }

      // As defined here: https://github.com/OpenGrok/OpenGrok/wiki/OpenGrok-web-services
      public class OpenGrokJsonSearchResponse {
         public List<OpenGrokJsonSearchResult> results { get; set; }
      }

      public class OpenGrokJsonSearchResult {
         public string path { get; set; }
         //public string directory { get; set; }
         //public string filename { get; set; }
         //public int lineno { get; set; }
         string _line;
         public string line {
            get { return _line; }
            set { _line = Encoding.UTF8.GetString(Convert.FromBase64String(value)); }
         }
      }
   }
}