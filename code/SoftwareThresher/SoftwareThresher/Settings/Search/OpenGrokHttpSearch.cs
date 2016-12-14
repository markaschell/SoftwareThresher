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

      // TODO - Does not always work need to figure out why - report as unknown with the link?
      public Date GetLastEditDate(Observation observation) {
         var url = GetHistoryUrl(observation);
         var response = webRequest.IssueRequest(url);
         var tableDetails = response.SelectNodes("//table[@id='revisions']/tbody/tr/td");

         // TODO - other checks or just catch any exception?  Do this other places?
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