using System;

namespace SoftwareThresher.Configurations {
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
   public class UsageNoteAttribute : Attribute {

      public string Note { get; private set; }

      public UsageNoteAttribute(string note) {
         Note = note;
      }
   }
}
