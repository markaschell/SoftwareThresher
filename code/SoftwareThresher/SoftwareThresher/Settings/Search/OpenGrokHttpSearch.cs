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

      public List<Observation> GetObservations(string location, string searchPattern) {
         throw new NotImplementedException();
      }

      public List<string> GetReferenceLine(Observation observation, string searchPattern) {
         throw new NotImplementedException();
      }

      public Date GetLastEditDate(Observation observation) {
         var url = GetHistoryUrl(observation);
         var response = webRequest.IssueRequest(url);
         var tableDetails = response.SelectNodes("//table[@id='revisions']/tbody/tr/td");

         if (tableDetails == null) {
            return Date.NullDate;
         }

         var dateString = tableDetails[2].InnerText;
         return new Date(DateTime.Parse(dateString));
      }

      public string GetHistoryUrl(Observation observation) {
         return $"{BaseLocation}/history{observation.SystemSpecificString}";
      }
   }
}