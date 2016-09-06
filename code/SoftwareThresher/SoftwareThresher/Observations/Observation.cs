namespace SoftwareThresher.Observations {
   public abstract class Observation {
      public virtual bool Failed { get; protected set; }

      public abstract string Name { get; }

      public abstract string Location { get; }

   }
}
