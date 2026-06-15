namespace Vergil.Pipeline;

public interface IPipelineBuilder
{
    IPipelineBuilder Add<TStage>()
        where TStage : IPipelineStage;
}
