namespace SoftwareThresher.Observations {
   public abstract class Observation {
      public virtual bool Passed { get; protected set; }

      public abstract string Name { get; }

      public abstract string Location { get; }

   }
}
