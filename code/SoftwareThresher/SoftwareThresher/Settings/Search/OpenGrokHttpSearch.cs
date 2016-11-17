using System;
using System.Collections.Generic;
using SoftwareThresher.Observations;
using SoftwareThresher.Utilities;

namespace SoftwareThresher.Settings.Search {

   public class OpenGrokHttpSearch : Search {

      readonly IWebRequest webRequest;

      public OpenGrokHttpSearch() {
         webRequest = new WebRequest();
      }

      public string BaseLocation { private get; set; }

      // TODO - ONLINE Implement
      public List<Observation> GetObservations(string location, string searchPattern) {
         throw new NotImplementedException();
      }

      // TODO - ONLINE Implement
      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         throw new NotImplementedException();
      }

      // TODO - ONLINE Test this and make sure it shows up in the report
      public Date GetLastEditDate(Observation observation)
      {
         var response = webRequest.IssueRequest(GetHistoryUrl(observation));
         var tableDetails = response.SelectNodes("//form/table/tbody/tr/td");

         var dateString = tableDetails[2].InnerText;
         return new Date(DateTime.Parse(dateString));
      }

      public string GetHistoryUrl(Observation observation)
      {
         return $"{BaseLocation}/history{observation.SystemSpecificString}";
      }
   }
}