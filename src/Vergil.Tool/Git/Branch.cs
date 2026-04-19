namespace Vergil.Tool.Git;

public class Branch(IRepository repository, Reference reference, string? canonicalName = null) : ReferenceWrapper<Commit>(repository, reference, canonicalName ?? reference.TargetIdentifier)
{
    public Commit? Tip => TargetObject;

    public ICommitLog Commits { get; }

    public virtual Branch? TrackedBranch { get; }

    protected override string Shorten()
    {
        if (CanonicalName.LooksLikeLocalBranch())
        {
            return CanonicalName.Substring(Reference.LocalBranchPrefix.Length);
        }

        if (CanonicalName.LooksLikeRemoteTrackingBranch())
        {
            return CanonicalName.Substring(Reference.RemoteTrackingBranchPrefix.Length);
        }

        throw new ArgumentException($"'{CanonicalName}' does not look like a valid branch name");
    }
}
