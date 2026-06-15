namespace Vergil;

public class VergilConfiguration : VergilBranchConfiguration
{
    public VergilConfiguration()
    {
        Match = "(?<BranchName>.+)";
        Label = "${BranchName}";
        Mode = IncrementMode.Tagged;
        Increment = VersionPart.Patch;
    }

    public string NextVersion { get; set; } = "0.0.0";

    public string TagPrefix { get; set; } = "v|V?";

    public List<VergilBranchConfiguration> Branhes { get; set; } = [];
}
