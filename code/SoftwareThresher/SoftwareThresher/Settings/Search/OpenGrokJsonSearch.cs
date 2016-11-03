using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using SoftwareThresher.Observations;

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

      // TODO - test this?
      string GetPathSearchPattern(string location, string searchPattern) {
         if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(searchPattern)) {
            return $"{location}+{searchPattern}";
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

         return parameters.Aggregate(string.Empty, (current, parameter) => $"{current}{ParameterJoin}{parameter}");
      }

      // TODO - move to another file and test
      public class OpenGrokParameter
      {
         readonly string label;
         readonly string value;

         public OpenGrokParameter(string label, string value) {
            this.label = label;
            this.value = value;
         }

         public override string ToString()
         {
            const string quotesString = "\"";

            var escapedValue = value.Replace(quotesString, $"\\{quotesString}");
            var quotes = escapedValue.Any(char.IsWhiteSpace) ? quotesString : string.Empty;

            return $"{label}={quotes}{escapedValue}{quotes}";
         }
      }
   }
}