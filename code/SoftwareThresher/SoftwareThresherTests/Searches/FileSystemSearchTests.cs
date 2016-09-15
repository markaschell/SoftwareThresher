using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher.Searches;
using SoftwareThresher.Utilities;

namespace SoftwareThresherTests.Searches {
   [TestClass]
   public class FileSystemSearchTests {
      ISystemFileReader reader;

      FileSystemSearch search;

      [TestInitialize]
      public void Setup() {
         reader = Substitute.For<ISystemFileReader>();

         search = new FileSystemSearch(reader);
      }
      [TestMethod]
      public void TestMethod1() {
      }
   }
}
