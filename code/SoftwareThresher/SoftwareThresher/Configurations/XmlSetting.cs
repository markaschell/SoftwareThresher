using System.Collections.Generic;

namespace SoftwareThresher.Configurations {
   public class XmlSetting : XmlNode {
      public string Name { get; set; }

      // TODO - Change this to a enum?  And the allowable values would be the class names exactly and use that in reporting the usage
      public string Type { get; set; }
      public List<XmlAttribute> Attributes { get; set; }

      public XmlSetting() {
         Attributes = new List<XmlAttribute>();
      }
   }
}
