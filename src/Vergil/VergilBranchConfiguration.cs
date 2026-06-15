namespace Vergil;

public class VergilBranchConfiguration
{
    public string? Match { get; set; }

    public string? Label { get; set; }

    public IncrementMode? Mode { get; set; }

    public VersionPart? Increment { get; set; }
}
