using Vergil.Versioning;

namespace Vergil.Pipeline;

public interface IPipeline
{
    void Execute(VersionContext context);
}
