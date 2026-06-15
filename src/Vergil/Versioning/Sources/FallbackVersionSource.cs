using Vergil.Git;

namespace Vergil.Versioning.Sources;

public class FallbackVersionSource(IRepository repository) : IVersionSource
{
    private const string FallbackVersion = "0.0.0";

    public IEnumerable<TagVersion> GetVersions()
    {
        if (repository.Head.Tip == null)
        {
            throw new InvalidOperationException("Unable to fetch HEAD");
        }

        yield return new TagVersion(repository.Head.Tip.Sha, SemanticVersion.Parse(FallbackVersion));
    }
}
