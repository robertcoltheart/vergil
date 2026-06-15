using Vergil.Versioning;

namespace Vergil.Pipeline;

public interface IPipelineStage
{
    void Execute(VersionContext context);
}
