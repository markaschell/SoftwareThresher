using System.Net;
using HtmlAgilityPack;

namespace SoftwareThresher.Utilities {
   public interface IWebRequest
   {
      HtmlNode IssueRequest(string url);
   }

   public class WebRequest : IWebRequest
   {
      public HtmlNode IssueRequest(string url) {
         ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
         var web = new HtmlWeb();
         return web.Load(url, "GET").DocumentNode;
      }
   }
}
