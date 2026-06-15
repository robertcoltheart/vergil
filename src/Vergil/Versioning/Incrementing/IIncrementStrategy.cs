namespace Vergil.Versioning.Incrementing;

public interface IIncrementStrategy
{
    bool ShouldIncrement(VersionContext context);

    SemanticVersion Increment(VersionContext context);
}
