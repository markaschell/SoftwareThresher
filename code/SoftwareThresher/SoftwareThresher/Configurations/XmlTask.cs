﻿using System.Collections.Generic;

namespace SoftwareThresher.Configurations {
   public class XmlTask : XmlNode {
      public string Name { get; set; }
      public List<XmlAttribute> Attributes { get; set; }

      public XmlTask() {
         Attributes = new List<XmlAttribute>();
      }
   }
}
