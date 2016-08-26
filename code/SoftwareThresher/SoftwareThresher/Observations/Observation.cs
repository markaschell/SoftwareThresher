namespace SoftwareThresher.Observations
{
    public interface Observation
    {
        bool Passed { get; }

        string Name { get; }
        string Location { get; }
    }
}
