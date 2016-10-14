using System;

namespace SoftwareThresher.Configurations {
   [AttributeUsage(AttributeTargets.Property)]
   public class UsageNoteAttribute : Attribute {

      public string Note { get; private set; }

      public UsageNoteAttribute(string note) {
         Note = note;
      }
   }
}
