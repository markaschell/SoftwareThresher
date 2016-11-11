using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftwareThresher.Configurations {
   public interface IAttributeLoader
   {
      object SetAttributes(List<XmlAttribute> attributes, object objectToSetAttributesOn);
   }

   public class AttributeLoader : IAttributeLoader
   {
      public object SetAttributes(List<XmlAttribute> attributes, object objectToSetAttributesOn) {
         foreach (var attribute in attributes) {
            SetAttribute(attribute, objectToSetAttributesOn);
         }

         return objectToSetAttributesOn;
      }

      static void SetAttribute(XmlAttribute attribute, object objectToSetAttributesOn) {
         var property = GetProperty(attribute, objectToSetAttributesOn);
         SetProperty(property, attribute, objectToSetAttributesOn);
      }

      static PropertyInfo GetProperty(XmlAttribute attribute, object objectToSetAttributesOn) {
         var property = objectToSetAttributesOn.GetType().GetProperty(attribute.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

         if (property == null) {
            ThrowAttributeNotSupportedException(attribute, objectToSetAttributesOn.GetType().Name);
         }

         return property;
      }

      static void ThrowAttributeNotSupportedException(XmlAttribute attribute, string className) {
         throw new NotSupportedException($"{attribute.Name} is not a supported attribute for {className}.");
      }

      static void SetProperty(PropertyInfo property, XmlAttribute attribute, object objectToSetAttributesOn) {
         try {
            property.SetValue(objectToSetAttributesOn, attribute.Value);
         }
         catch (Exception e) {
            if (e.Message == "Property set method not found.") {
               ThrowAttributeNotSupportedException(attribute, objectToSetAttributesOn.GetType().Name);
            }

            throw new Exception($"{attribute.Value} is an invalid value for attribute {attribute.Name}.");
         }
      }
   }
}
