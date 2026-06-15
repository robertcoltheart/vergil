namespace Vergil.Git;

public abstract class Reference(string canonicalName, string targetIdentifier) : IEquatable<Reference>
{
    public const string LocalBranchPrefix = "refs/heads/";

    public const string RemoteTrackingBranchPrefix = "refs/remotes/";

    public const string TagPrefix = "refs/tags/";

    public string CanonicalName { get; } = canonicalName;

    public string TargetIdentifier { get; } = targetIdentifier;

    public bool IsLocalBranch => CanonicalName.LooksLikeLocalBranch();

    public bool IsRemoteTrackingBranch => CanonicalName.LooksLikeRemoteTrackingBranch();

    public bool IsTag => CanonicalName.LooksLikeTag();

    public abstract DirectReference? ResolveToDirectReference();

    public bool Equals(Reference? other)
    {
        return other?.CanonicalName == CanonicalName && other.TargetIdentifier == TargetIdentifier;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Reference);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CanonicalName, TargetIdentifier);
    }

    public override string ToString()
    {
        return CanonicalName;
    }
}
