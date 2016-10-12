using System.Collections.Generic;

namespace SoftwareThresher.Configurations {
   public interface XmlNode {
      string Name { get; set; }
      List<XmlAttribute> Attributes { get; set; }
   }
}
