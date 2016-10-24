using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher;

namespace SoftwareThresherIntegrationTests {
   [TestClass]
   public class ProgramTests {
      [TestMethod]
      public void Program_EnsureUsageTextDoesNotError() {
         Program.Main(new string[0]);
      }
   }
}
