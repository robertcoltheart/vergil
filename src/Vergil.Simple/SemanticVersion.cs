namespace Vergil.Simple;

public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>
{
    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public IEnumerable<string> PrereleaseLabels { get; }

    public bool IsPrerelease { get; }

    public IEnumerable<string> Metadata { get; }

    public static SemanticVersion Parse(string value)
    {
        return new SemanticVersion();
    }

    public static bool TryParse(string value, out SemanticVersion version)
    {
        version = new SemanticVersion();

        return true;
    }

    public bool Equals(SemanticVersion other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(SemanticVersion other)
    {
        throw new NotImplementedException();
    }
}
