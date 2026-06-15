namespace Vergil.Versioning.Incrementing;

public class TaggedIncrementStrategy : IncrementStrategy
{
    protected override IncrementMode Mode => IncrementMode.Tagged;

    public override SemanticVersion Increment(VersionContext context)
    {
        var mode = context.Branch.Mode ?? context.Configuration.Mode;

        if (mode != IncrementMode.Tagged)
        {
            return null;
        }

        throw new NotImplementedException();
    }
}
