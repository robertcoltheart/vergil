namespace Vergil.Versioning;

public class VersionMetadata
{
    public string? PreReleaseLabel { get; set; }

    public Dictionary<string, string> Variables { get; } = [];
}
