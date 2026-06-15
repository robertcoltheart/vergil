using Vergil.Versioning;

namespace Vergil.Pipeline;

public class Pipeline(IServiceProvider provider, Type[] stageTypes) : IPipeline
{
    public void Execute(VersionContext context)
    {
        foreach (var stageType in stageTypes)
        {
            if (provider.GetService(stageType) is not IPipelineStage stage)
            {
                throw new InvalidOperationException("Invalid stage type registered");
            }

            stage.Execute(context);
        }
    }
}
