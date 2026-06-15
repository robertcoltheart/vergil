using Vergil.Pipeline;
using Vergil.Versioning.Sources;

namespace Vergil.Versioning.Stages;

public class BaseVersionStage(IEnumerable<IVersionSource> sources) : IPipelineStage
{
    public void Execute(VersionContext context)
    {
        context.SetBaseVersion(GetBaseVersion());
    }

    private TagVersion GetBaseVersion()
    {
        var version = sources
            .SelectMany(x => x.GetVersions())
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (version == null)
        {
            throw new InvalidOperationException("Base version cannot be calculated");
        }

        return version;
    }
}
