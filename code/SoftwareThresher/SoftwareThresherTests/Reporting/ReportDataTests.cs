using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Reporting;

namespace SoftwareThresherTests.Reporting {
   [TestClass]
   public class ReportDataTests {
      ReportData reportData;

      [TestInitialize]
      public void Setup() {
         reportData = new ReportData();
      }

      [TestMethod]
      public void GetFileNameWithoutExtesion_RemovesExtenstionAndAddsDateTime() {
         const string configurationFilename = "input";

         var filename = reportData.GetFileNameWithoutExtesion(configurationFilename + ".xml");

         Assert.IsTrue(filename.StartsWith(configurationFilename + "_"));
      }

      [TestMethod]
      public void GetFileNameWithoutExtesion_RemovesDirectory() {
         const string configurationFilename = "input";

         var filename = reportData.GetFileNameWithoutExtesion("diretory\\" + configurationFilename);

         Assert.IsTrue(filename.StartsWith(configurationFilename + "_"));
      }

      [TestMethod]
      public void GetFileNameWithoutExtesion_NoExtenstionReturnsSameValue() {
         const string configurationFilename = "input";

         var filename = reportData.GetFileNameWithoutExtesion(configurationFilename);

         Assert.IsTrue(filename.Contains(configurationFilename + "_"));
      }

      [TestMethod]
      public void GetFileNameWithoutExtesion_AddsDateTime() {
         const string configurationFilename = "input";
         var now = DateTime.Now;

         var filename = reportData.GetFileNameWithoutExtesion(configurationFilename + ".xml");

         // filename + "_" + date and time down to millisecond
         Assert.AreEqual(5 + 1 + 8 + 9, filename.Length);
         Assert.IsTrue(filename.Contains("_" + now.Year.ToString("0000") + now.Month.ToString("00") + now.Day.ToString("00") + now.Hour.ToString("00") + now.Minute.ToString("00")));
      }
   }
}
