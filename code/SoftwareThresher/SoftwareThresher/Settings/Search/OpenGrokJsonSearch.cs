using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Settings.Search {

   public class OpenGrokJsonSearch : Search {

      const string TextSearchParameterLabel = "freetext=";
      //const string DefinitionSearchParameterLabel = "def=";
      //const string SymbolSearchParameterLabel = "symbol=";
      const string PathSearchParameterLabel = "path=";
      //const string HistorySearchParameterLabel = "hist=";
      const string ParameterJoin = "&";

      public string BaseLocation { private get; set; }

      // TODO - appears to be only returning 1000 at a time if only searching by path
      // TODO - Find a better way to search by ".cs"
      // TODO - use location?
      public List<Observation> GetObservations(string location, string searchPattern) {
         // TODO - Could we use "file://" to access svn server - wonder which is faster
         // TODO - I do not think we want a FileObservation here - create a new class
         return GetResults(PathSearchParameterLabel + searchPattern).ConvertAll(r => (Observation)new FileObservation(r.path));
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         return GetResults(PathSearchParameterLabel + observation + ParameterJoin + TextSearchParameterLabel + searchPattern).ConvertAll(r => r.line);
      }

      List<OpenGrokJsonSearchResult> GetResults(string parameters) {
         var request = WebRequest.Create(BaseLocation + "/json?" + parameters);

         using (var response = request.GetResponse()) {
            var serializer = new DataContractJsonSerializer(typeof(OpenGrokJsonSearchResponse));
            var searchResponse = (OpenGrokJsonSearchResponse)serializer.ReadObject(response.GetResponseStream());

            return searchResponse.results;
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