namespace Vergil.Versioning.Incrementing;

public class ExplicitTagIncrementStrategy : IIncrementStrategy
{
    public int Order => int.MinValue;

    public bool ShouldIncrement(VersionContext context)
    {
        return context.BaseVersion.Sha == context.Repository.Head.Tip?.Sha;
    }

    public SemanticVersion Increment(VersionContext context)
    {
        return context.BaseVersion.Version;
    }
}
