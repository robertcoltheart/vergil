using Jab;
using Vergil.Git;
using Vergil.Pipeline;
using Vergil.Versioning;
using Vergil.Versioning.Incrementing;
using Vergil.Versioning.Sources;
using Vergil.Versioning.Stages;

namespace Vergil;

[ServiceProvider]
[Singleton<IIncrementStrategy, ExplicitTagIncrementStrategy>]
[Singleton<IIncrementStrategy, TaggedIncrementStrategy>]
[Singleton<IIncrementStrategy, ContinuousIncrementStrategy>]
[Singleton<IVersionSource, FallbackVersionSource>]
[Singleton<IVersionSource, ConfigurationVersionSource>]
[Singleton<IVersionSource, TaggedVersionSource>]
[Singleton<VersionCalculator>]
[Singleton<BaseVersionStage>]
[Singleton<BranchConfigurationStage>]
[Singleton<CommitHeightStage>]
[Singleton<IncrementVersionStage>]
[Singleton<VergilConfiguration>(Instance = nameof(Configuration))]
[Singleton<IRepository>(Factory = nameof(GetRepository))]
[Singleton<IPipeline>(Factory = nameof(BuildPipeline))]
internal partial class ServiceProvider(string path, VergilConfiguration configuration)
{
    private VergilConfiguration Configuration => configuration;

    private IRepository GetRepository()
    {
        return new Repository(path);
    }

    private IPipeline BuildPipeline()
    {
        return new PipelineBuilder(this)
            .Add<BranchConfigurationStage>()
            .Add<BaseVersionStage>()
            .Add<CommitHeightStage>()
            .Add<IncrementVersionStage>()
            .Build();
    }
}
