using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareThresher.Configurations;
using SoftwareThresherTests.Configurations.TypeStubs;

namespace SoftwareThresherTests.Configurations {
   [TestClass]
   public class AttributeLoaderTests {
      IAttributeLoader attributeLoader;

      [TestInitialize]
      public void Setup() {
         attributeLoader = new AttributeLoader();
      }

      [TestMethod]
      public void SetAttributes_SetsAttributes() {
         const string attribute1Value = "one";
         const string attribute2Value = "two";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "Attribute2", Value = attribute2Value } ,
                                                   new XmlAttribute { Name = "Attribute1", Value = attribute1Value } };

         var result = (TestTask)attributeLoader.SetAttributes(attributes, new TestTask());

         Assert.AreEqual(attribute1Value, result.Attribute1);
         Assert.AreEqual(attribute2Value, result.Attribute2);
      }

      [TestMethod]
      public void SetAttributes_SetsBaseClassAttribute() {
         const string header = "one";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "ReportHeaderText", Value = header } };

         var result = (TestTask)attributeLoader.SetAttributes(attributes, new TestTask());

         Assert.AreEqual(header, result.ReportHeaderText);
      }

      [TestMethod]
      public void SetAttributes_InvalidAttributeName_ThrowsException() {
         const string invalidPropertyName = "BAD NAME";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = invalidPropertyName } };

         try {
            attributeLoader.SetAttributes(attributes, new TestTask());

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(invalidPropertyName + " is not a supported attribute for TestTask.", e.Message);
         }
      }

      [TestMethod]
      public void SetAttributes_InvalidAttributeValue_ThrowsException() {
         const string attributeValue = "one";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "IntAttribute", Value = attributeValue } };

         try {
            attributeLoader.SetAttributes(attributes, new TestTask());

            Assert.Fail("Should have thrown an exception.");
         }
         catch (Exception e) {
            Assert.AreEqual(attributeValue + " is an invalid value for attribute IntAttribute.", e.Message);
         }
      }

      [TestMethod]
      public void SetAttributes_PrivateAttribute_ThrowsException() {
         var task = new TestTask();
         var privateAttribute = task.GetType().GetProperty("PrivateAttribute", BindingFlags.Instance | BindingFlags.NonPublic).Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = privateAttribute } };

         try {
            attributeLoader.SetAttributes(attributes, new TestTask());

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(privateAttribute + " is not a supported attribute for TestTask.", e.Message);
         }
      }

      [TestMethod]
      public void SetAttributes_AttributeWithNoSet_ThrowsException() {
         var task = new TestTask();
         var reportTileTaskGetAttribute = task.GetType().GetProperty("GetOnlyAttribute").Name;
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = reportTileTaskGetAttribute } };

         try {
            attributeLoader.SetAttributes(attributes, new TestTask());

            Assert.Fail("Should have thrown an exception.");
         }
         catch (NotSupportedException e) {
            Assert.AreEqual(reportTileTaskGetAttribute + " is not a supported attribute for TestTask.", e.Message);
         }
      }

      [TestMethod]
      public void Load_SetsAttributeIgnoresCase() {
         const string attributeValue = "value";
         var attributes = new List<XmlAttribute> { new XmlAttribute { Name = "attribute1", Value = attributeValue } };

         var result = (TestTask)attributeLoader.SetAttributes(attributes, new TestTask());

         Assert.AreEqual(attributeValue, result.Attribute1);
      }
   }
}
