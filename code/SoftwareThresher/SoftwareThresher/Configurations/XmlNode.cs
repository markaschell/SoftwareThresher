using System.Collections.Generic;

namespace SoftwareThresher.Configurations {
   public class XmlNode {
      public string Name { get; set; }
      public List<XmlAttribute> Attributes { get; set; }

      public XmlNode() {
         Attributes = new List<XmlAttribute>();
      }
   }
}
