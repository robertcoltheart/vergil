using Vergil.Pipeline;
using Vergil.Versioning.Incrementing;

namespace Vergil.Versioning.Stages;

public class IncrementVersionStage(IEnumerable<IIncrementStrategy> strategies) : IPipelineStage
{
    public void Execute(VersionContext context)
    {
        context.CalculatedVersion = IncrementVersion(context);
    }

    private SemanticVersion IncrementVersion(VersionContext context)
    {
        var version = strategies
            .Where(x => x.ShouldIncrement(context))
            .Select(x => x.Increment(context))
            .FirstOrDefault();

        return version ??
               throw new InvalidOperationException("Version cannot be calculated with current configuration set");
    }
}
