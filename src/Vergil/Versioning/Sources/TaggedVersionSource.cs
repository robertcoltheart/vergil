using System.Text.RegularExpressions;
using Vergil.Git;

namespace Vergil.Versioning.Sources;

public class TaggedVersionSource(IRepository repository, VergilConfiguration configuration) : IVersionSource
{
    public IEnumerable<TagVersion> GetVersions()
    {
        return repository.Tags
            .Select(x => new { Tag = x, Name = GetTagName(x.FriendlyName) })
            .Where(x => SemanticVersion.TryParse(x.Name, out _))
            .Select(x => new TagVersion(x.Tag.Target!.Sha, SemanticVersion.Parse(x.Name)));
    }

    private string GetTagName(string value)
    {
        if (string.IsNullOrEmpty(configuration.TagPrefix))
        {
            return value;
        }

        return Regex.Replace(value, $"^({configuration.TagPrefix})", string.Empty);
    }
}
