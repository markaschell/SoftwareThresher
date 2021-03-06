﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

// TODO On Upgrade of Opengrok (current version ) check the following:
//   - if formatting is still in line value
//   - if searching by path with an extension works.  ex - ".cs"
//   - if hist can be searched
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

      readonly OpenGrokHttpSearch openGrokHttpSearch;

      public OpenGrokJsonSearch()
      {
         openGrokHttpSearch = new OpenGrokHttpSearch();
      }

      public List<Observation> GetObservations(string location, string searchPattern) {
         var parameters = new List<OpenGrokParameter> { new OpenGrokParameter(PathSearchParameterLabel, GetPathSearchPattern(location, searchPattern)) };

         return GetResults(parameters).ConvertAll(r => (Observation)new OpenGrokObservation(r.directory, r.filename, this));
      }

      static string GetPathSearchPattern(string location, string searchPattern) {
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
         var request = System.Net.WebRequest.Create($"{BaseLocation}/json?{parameterString}");

         using (var response = request.GetResponse()) {
            var serializer = new DataContractJsonSerializer(typeof(OpenGrokJsonSearchResponse));
            var searchResponse = (OpenGrokJsonSearchResponse)serializer.ReadObject(response.GetResponseStream());

            if (searchResponse.resultcount == searchResponse.maxresults) {
               throw new Exception($"Maximum Opengrok results reached: {searchResponse.maxresults}");
            }

            return searchResponse.results;
         }
      }

      static string BuildParameterString(ICollection<OpenGrokParameter> parameters) {
         parameters.Add(new OpenGrokParameter(MaxResultsParameterLabel, MaxResults));

         return parameters.Aggregate(string.Empty, (current, parameter) => $"{current}{ParameterJoin}{parameter}");
      }

      public Date GetLastEditDate(Observation observation)
      {
         SetPropertiesOnHttpSearch();
         return openGrokHttpSearch.GetLastEditDate(observation);
      }

      void SetPropertiesOnHttpSearch()
      {
         openGrokHttpSearch.BaseLocation = BaseLocation;
      }

      public string GetHistoryUrl(Observation observation)
      {
         SetPropertiesOnHttpSearch();
         return openGrokHttpSearch.GetHistoryUrl(observation);
      }
   }
}