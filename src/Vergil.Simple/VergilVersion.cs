namespace Vergil.Simple;

public class VergilVersion
{
    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public string PreRelease { get; } = string.Empty;

    public string SemanticVersion { get; } = string.Empty;

    public string AssemblyVersion => $"{Major}.0.0.0";

    public string FileVersion => $"{Major}.{Minor}.{Patch}.0";

    public string InformationalVersion => SemanticVersion;

    public string PackageVersion => $"{Major}.{Minor}.{Patch}-{PreRelease}";

    public string Version => PackageVersion;
}
