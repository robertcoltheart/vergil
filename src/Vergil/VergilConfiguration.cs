namespace Vergil;

public class VergilConfiguration
{
    public string TagPrefix { get; set; } = "v";

    public string? Path { get; set; }

    public VersionPart Incrementing { get; set; } = VersionPart.Metadata;
}
