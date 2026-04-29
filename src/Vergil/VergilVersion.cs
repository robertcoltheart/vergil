namespace Vergil;

public class VergilVersion
{
    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public string BranchName { get; }

    public string Sha { get; }

    public string ShortSha { get; }

    public string PreRelease { get; } = string.Empty;

    public string BuildMetadata { get; } = string.Empty;

    public string SemanticVersion { get; } = string.Empty;

    public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";

    public string AssemblyVersion => $"{Major}.0.0.0";

    public string FileVersion => $"{Major}.{Minor}.{Patch}.0";

    public string InformationalVersion => SemanticVersion;

    public string PackageVersion => $"{Major}.{Minor}.{Patch}-{PreRelease}";

    public string Version => PackageVersion;

    public VergilVersion Increment(VersionPart part)
    {
        return this;
    }

    public VergilVersion WithBranchLabel()
    {
        return this;
    }

    public VergilVersion WithLabel(string label)
    {
        return this;
    }
}
