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
      const string MaxResultsParameterLabel = "maxresults=";
      const string ParameterJoin = "&";
      const int MaxResults = 1000000; // In theroy could go as high as java max Integer

      public string BaseLocation { private get; set; }

      // TODO - fix the filter for file search - Do I need to????
      // TODO - use location?
      public List<Observation> GetObservations(string location, string searchPattern) {
         return GetResults($"{PathSearchParameterLabel}\"{searchPattern}\"").ConvertAll(r => (Observation)new OpenGrokObservation(r.directory, r.filename));
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         return GetResults($"{PathSearchParameterLabel}\"{observation}\"{ParameterJoin}{TextSearchParameterLabel}\"{searchPattern}\"").ConvertAll(r => r.line);
      }

      List<OpenGrokJsonSearchResult> GetResults(string parameters) {
         var request = WebRequest.Create($"{BaseLocation}/json?{parameters}{ParameterJoin}{MaxResultsParameterLabel}{MaxResults}");

         using (var response = request.GetResponse()) {
            var serializer = new DataContractJsonSerializer(typeof(OpenGrokJsonSearchResponse));
            var searchResponse = (OpenGrokJsonSearchResponse)serializer.ReadObject(response.GetResponseStream());

            if (searchResponse.resultcount == searchResponse.maxresults)
            {
               throw new Exception($"Maximum Opengrok results reached: {searchResponse.maxresults}");
            }

            return searchResponse.results;
         }
      }

      // As defined here: https://github.com/OpenGrok/OpenGrok/wiki/OpenGrok-web-services
      public class OpenGrokJsonSearchResponse {
         //public string duration { get; set; }
         public string resultcount { get; set; }
         public string maxresults { get; set; }
         public List<OpenGrokJsonSearchResult> results { get; set; }
      }

      public class OpenGrokJsonSearchResult {
         public string directory { get; set; }
         public string filename { get; set; }
         //public int lineno { get; set; }
         string _line;
         public string line {
            get { return _line; }
            set { _line = Encoding.UTF8.GetString(Convert.FromBase64String(value)); }
         }
         //public string path { get; set; }
      }
   }
}