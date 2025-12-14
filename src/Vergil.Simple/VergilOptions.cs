namespace Vergil.Simple;

public class VergilOptions
{
    public string TagPrefix { get; set; } = "v";

    public string BuildMetadata { get; set; } = string.Empty;

    public string PreReleaseIdentifier { get; set; } = string.Empty;
}
