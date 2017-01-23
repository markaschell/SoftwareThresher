using System.Net;
using System.Net.Http;
using HtmlAgilityPack;

namespace SoftwareThresher.Utilities {
   public interface IWebRequest {
      HtmlNode IssueRequest(string url);
   }

   public class WebRequest : IWebRequest {
      public HtmlNode IssueRequest(string url) {
         // TODO - check out .Net.HtttpClient?
         ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
         var web = new HtmlWeb();

         return web.Load(url, "GET").DocumentNode;
      }
   }
}
