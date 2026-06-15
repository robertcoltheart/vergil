namespace Vergil.Versioning.Incrementing;

public abstract class IncrementStrategy : IIncrementStrategy
{
    protected abstract IncrementMode Mode { get; }

    public bool ShouldIncrement(VersionContext context)
    {
        return GetMode(context) == Mode;
    }

    public abstract SemanticVersion Increment(VersionContext context);

    protected IncrementMode? GetMode(VersionContext context)
    {
        return context.Branch.Mode ?? context.Configuration.Mode;
    }

    protected string GetLabel(VersionContext context)
    {
        var label = context.Branch.Label ?? context.Configuration.Label;

        return string.IsNullOrEmpty(label)
            ? string.Empty
            : context.Repository.Head.FriendlyName.Escaped();
    }
}
