using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Settings {

   public class OpenGrokJsonSearch : Search {

      const string TextSearchParameterLabel = "freetext=";
      //const string DefinitionSearchParameterLabel = "def=";
      //const string SymbolSearchParameterLabel = "symbol=";
      const string PathSearchParameterLabel = "path=";
      //const string HistorySearchParameterLabel = "hist=";
      const string ParameterJoin = "&";

      // TODO - use this
      public string BaseLocation { private get; set; }

      public List<Observation> GetObservations(string directory, string searchPattern) {
         // TODO - Add example?
         // TODO - Rename directory include the task parameter?
         // TODO - use directory to assign the path variable?
         // TODO - Add type as a parameter the the task or define it for all tasks?  Play with injection and how that will work with reflection....Assign parameters in Task or pass into the constructor?  Should we default all of the items in the base?
         // TODO - Could we use "file://" in the directory to search locally and just use the same Search?  Probably not because of the different return types.  Use that to access svn server - wonder which is faster

         // TODO - I do not think we want a FileObservation here
         return GetResults(TextSearchParameterLabel + searchPattern).ConvertAll(r => (Observation)new FileObservation(r.path));
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         return GetResults(PathSearchParameterLabel + observation + ParameterJoin + TextSearchParameterLabel + searchPattern).ConvertAll(r => r.line);
      }

      List<OpenGrokJsonSearchResult> GetResults(string parameters) {
         // TODO - could opengrok be configured to a different location?  {host}/{webapp_name}/json?freetext - make the host a configuration setting?
         // TODO - configure the webapp_name - make this the location name?
         // Does we need to make this configurable at this time?
         var request = WebRequest.Create("http://opengrok/source/json?" + parameters);

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