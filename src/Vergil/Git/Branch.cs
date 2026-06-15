namespace Vergil.Git;

public class Branch(IRepository repository, Reference reference, string? canonicalName = null) : ReferenceWrapper<Commit>(repository, reference, canonicalName ?? reference.TargetIdentifier)
{
    public Commit? Tip => TargetObject;

    public ICommitLog Commits => repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = this });

    protected override string Shorten()
    {
        if (CanonicalName.LooksLikeLocalBranch())
        {
            return CanonicalName[Reference.LocalBranchPrefix.Length..];
        }

        if (CanonicalName.LooksLikeRemoteTrackingBranch())
        {
            return CanonicalName[Reference.RemoteTrackingBranchPrefix.Length..];
        }

        throw new ArgumentException($"'{CanonicalName}' does not look like a valid branch name");
    }
}
