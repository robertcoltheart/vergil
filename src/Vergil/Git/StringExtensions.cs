namespace Vergil.Git;

public static class StringExtensions
{
    public static bool LooksLikeLocalBranch(this string value)
    {
        return value.IsPrefixedBy(Reference.LocalBranchPrefix);
    }

    public static bool LooksLikeRemoteTrackingBranch(this string value)
    {
        return value.IsPrefixedBy(Reference.RemoteTrackingBranchPrefix);
    }

    public static bool LooksLikeTag(this string value)
    {
        return value.IsPrefixedBy(Reference.TagPrefix);
    }

    private static bool IsPrefixedBy(this string value, string prefix)
    {
        return value.StartsWith(prefix, StringComparison.Ordinal);
    }
}
