using System;

namespace SoftwareThresher {
   [AttributeUsage(AttributeTargets.Property)]
   public class UsageNoteAttribute : Attribute {

      public string Note { get; private set; }

      public UsageNoteAttribute(string note) {
         Note = note;
      }
   }
}
