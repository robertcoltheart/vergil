using System.Text.Json;

namespace Vergil;

public class VergilVersion
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public string AssemblySemFileVer => AssemblySemVer;

    public string AssemblySemVer => $"{MajorMinorPatch}.0";

    public required string BranchName { get; init; }

    // Number of commits since VersionSourceSha
    public int BuildMetadata => VersionSourceDistance;

    public string EscapedBranchName => BranchName
        .Replace('/', '-')
        .Replace('\\', '-')
        .Replace('.', '-');

    public string FullBuildMetadata => IsPreRelease
        ? $"{PreReleaseNumber}.Branch.{EscapedBranchName}.Sha.{Sha}"
        : $"Branch.{EscapedBranchName}.Sha.{Sha}";

    public string FullSemVer => $"{SemVer}+{FullBuildMetadata}";

    public required int Major { get; init; }

    public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";

    public required int Minor { get; init; }

    public required int Patch { get; init; }

    public required string PreReleaseLabel { get; init; }

    // Number of commits since current pre-release initial version
    public required int PreReleaseNumber { get; init; }

    public string PreReleaseTag => IsPreRelease
        ? $"{PreReleaseLabel}.{PreReleaseNumber}"
        : string.Empty;

    public string SemVer => IsPreRelease
        ? $"{MajorMinorPatch}-{PreReleaseTag}"
        : $"{MajorMinorPatch}";

    public required string Sha { get; init; }

    public string ShortSha => Sha[..7];

    // Number of commits since most recent version
    public required int VersionSourceDistance { get; init; }

    public required string VersionSourceIncrement { get; init; }

    public required string VersionSourceSemVer { get; init; }

    public required string VersionSourceSha { get; init; }

    private bool IsPreRelease => !string.IsNullOrEmpty(PreReleaseLabel);

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

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, Options);
    }
}
