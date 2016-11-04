using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Settings.Search;

namespace SoftwareThresherTests.Settings.Search {
   [TestClass]
   public class OpenGrokParameterTests {
      [TestMethod]
      public void ToString_NoFormatting()
      {
         const string label = "fin";
         const string value = "it";

         var parameter = new OpenGrokParameter(label, value);

         Assert.AreEqual("fin=it", parameter.ToString());
      }

      [TestMethod]
      public void ToString_ValueContainsAQuoteVariablee_EscapesQuote() {
         const string value = "it\"s";

         var parameter = new OpenGrokParameter("one", value);

         Assert.IsTrue(parameter.ToString().EndsWith("=it\\\"s"));
      }
      [TestMethod]
      public void ToString_ValueContainsSpaces_PutsQuotesAroundTheValue() {
         const string value = "it is";

         var parameter = new OpenGrokParameter("one", value);

         Assert.IsTrue(parameter.ToString().EndsWith("=\"it is\""));
      }
   }
}
