using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using SoftwareThresher.Observations;

// TODO - Add to the usage - Current searching does not support file extentions very well so we have to filter out until then
// On Upgrade of Opengrok (current version ) check the following:
//   - if formatting is still in line value
//   - if searching by path with an extension works.  ex - ".cs"
namespace SoftwareThresher.Settings.Search {

   public class OpenGrokJsonSearch : Search {

      const string TextSearchParameterLabel = "freetext";
      //const string DefinitionSearchParameterLabel = "def";
      //const string SymbolSearchParameterLabel = "symbol";
      const string PathSearchParameterLabel = "path";
      //const string HistorySearchParameterLabel = "hist";
      const string MaxResultsParameterLabel = "maxresults";
      const string ParameterJoin = "&";
      const string MaxResults = "1000000"; // In theroy could go as high as java max Integer

      public string BaseLocation { private get; set; }

      public List<Observation> GetObservations(string location, string searchPattern) {
         var parameters = new List<OpenGrokParameter> { new OpenGrokParameter(PathSearchParameterLabel, GetPathSearchPattern(location, searchPattern)) };

         return GetResults(parameters).ConvertAll(r => (Observation)new OpenGrokObservation(r.directory, r.filename));
      }

      string GetPathSearchPattern(string location, string searchPattern) {
         if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(searchPattern)) {
            return $"{location} {searchPattern}";
         }

         return !string.IsNullOrEmpty(location) ? location : searchPattern;
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         var parameters = new List<OpenGrokParameter>
         {
            new OpenGrokParameter(PathSearchParameterLabel, observation.SystemSpecificString),
            new OpenGrokParameter(TextSearchParameterLabel, searchPattern)
         };

         return GetResults(parameters).Where(r => !string.IsNullOrEmpty(r.lineno)).Select(r => r.line).ToList();
      }

      List<OpenGrokJsonSearchResult> GetResults(ICollection<OpenGrokParameter> parameters) {
         var parameterString = BuildParameterString(parameters);
         var request = WebRequest.Create($"{BaseLocation}/json?{parameterString}");

         using (var response = request.GetResponse()) {
            var serializer = new DataContractJsonSerializer(typeof(OpenGrokJsonSearchResponse));
            var searchResponse = (OpenGrokJsonSearchResponse)serializer.ReadObject(response.GetResponseStream());

            if (searchResponse.resultcount == searchResponse.maxresults) {
               throw new Exception($"Maximum Opengrok results reached: {searchResponse.maxresults}");
            }

            return searchResponse.results;
         }
      }

      // TODO - test any of this? - move to another class
      static string BuildParameterString(ICollection<OpenGrokParameter> parameters) {
         parameters.Add(new OpenGrokParameter(MaxResultsParameterLabel, MaxResults));

         return parameters.Aggregate(string.Empty, (current, parameter) => current + ParamaterString(parameter, string.IsNullOrEmpty(current)));
      }

      static string ParamaterString(OpenGrokParameter parameter, bool firstParameter) {
         const string quotes = "\"";

         var parameterJoin = firstParameter ? string.Empty : ParameterJoin;
         var value = parameter.Value.Replace(quotes, $"\\{quotes}");

         return $"{parameterJoin}{parameter.Label}={quotes}{value}{quotes}";
      }

      public class OpenGrokParameter {
         public OpenGrokParameter(string label, string value) {
            Label = label;
            Value = value;
         }

         public string Label { get; }
         public string Value { get; }
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
         public string lineno { get; set; }
         string _line;
         public string line {
            get { return _line; }
            set {
               var readableString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
               _line = readableString.Replace("</b>", string.Empty);
            }
         }
         //public string path { get; set; }
      }
   }
}