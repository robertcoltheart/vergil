namespace Vergil;

public class VergilVersionBuilder
{
    public VergilVersionBuilder ForPath(string path)
    {
        return this;
    }

    public VergilVersionBuilder WithTagPrefix(string prefix)
    {
        return this;
    }

    public VergilVersionBuilder WithLabel(string label)
    {
        return this;
    }

    public VergilVersionBuilder WithBranchAsLabel()
    {
        return this;
    }

    public VergilVersionBuilder Incrementing(VersionPart part)
    {
        return this;
    }

    public VergilVersionBuilder AddBranchDefinition(string pattern)
    {
        return this;
    }

    public VergilVersion Build()
    {
        return null;
    }
}
