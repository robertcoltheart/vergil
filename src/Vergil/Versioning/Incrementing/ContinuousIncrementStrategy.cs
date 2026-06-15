namespace Vergil.Versioning.Incrementing;

public class ContinuousIncrementStrategy : IncrementStrategy
{
    protected override IncrementMode Mode => IncrementMode.Continuous;

    public override SemanticVersion Increment(VersionContext context)
    {
        var increment = context.Branch.Increment ?? context.Configuration.Increment;

        if (increment == null)
        {
            throw new InvalidOperationException("Unable to increment version");
        }

        context.Metadata.PreReleaseLabel = GetLabel(context);

        var version = context.BaseVersion.Version;

        return increment switch
        {
            VersionPart.Major => version.WithMajor(version.Major + context.CommitHeight),
            VersionPart.Minor => version.WithMinor(version.Minor + context.CommitHeight),
            VersionPart.Patch => version.WithPatch(version.Patch + context.CommitHeight),
            _ => version
        };
    }
}
