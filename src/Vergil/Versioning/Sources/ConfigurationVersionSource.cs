using Vergil.Git;

namespace Vergil.Versioning.Sources;

public class ConfigurationVersionSource(IRepository repository, VergilConfiguration configuration) : IVersionSource
{
    public IEnumerable<TagVersion> GetVersions()
    {
        if (string.IsNullOrEmpty(configuration.NextVersion))
        {
            yield break;
        }

        if (repository.Head.Tip == null)
        {
            throw new InvalidOperationException("Unable to fetch HEAD");
        }

        yield return new TagVersion(
            repository.Head.Tip.Sha,
            SemanticVersion.Parse(configuration.NextVersion));
    }
}
