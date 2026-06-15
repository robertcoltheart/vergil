namespace Vergil.Pipeline;

public class PipelineBuilder(IServiceProvider provider)
{
    private readonly List<Type> stages = [];

    public PipelineBuilder Add<TStage>()
        where TStage : IPipelineStage
    {
        stages.Add(typeof(TStage));

        return this;
    }

    public IPipeline Build()
    {
        return new Pipeline(provider, stages.ToArray());
    }
}
