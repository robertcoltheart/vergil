using Vergil.Git;
using Vergil.Pipeline;

namespace Vergil.Versioning;

public class VersionCalculator(IRepository repository, IPipeline pipeline, VergilConfiguration configuration)
{
    public VergilVersion Calculate()
    {
        // 1. Find matcing branch configuration
        // 2. Calculate base version (from tag, next-version or fallback (0.0.0))
        // 3. Get next version using mode and increment config values

        var tip = repository.Head.Tip;

        if (tip == null)
        {
            throw new InvalidOperationException("Cannot obtain HEAD");
        }

        var context = new VersionContext(repository, configuration);

        pipeline.Execute(context);

        if (context.CalculatedVersion == null)
        {
            throw new InvalidOperationException("Unable to calculate version");
        }

        return new VergilVersion
        {
            BranchName = repository.Head.FriendlyName,
            Major = context.CalculatedVersion.Major,
            Minor = context.CalculatedVersion.Minor,
            Patch = context.CalculatedVersion.Patch,
            Sha = tip.Id.Sha,
            PreReleaseLabel = context.Metadata.PreReleaseLabel ?? string.Empty,
            PreReleaseNumber = 0,
            VersionSourceDistance = context.CommitHeight,
            VersionSourceIncrement = string.Empty,
            VersionSourceSemVer = context.BaseVersion.Version.ToString(),
            VersionSourceSha = context.BaseVersion.Sha
        };
    }
}
