using Vergil.Git;

namespace Vergil.Versioning;

public class VersionContext(IRepository repository, VergilConfiguration configuration)
{
    public IRepository Repository { get; } = repository;

    public VergilConfiguration Configuration { get; } = configuration;

    public VergilBranchConfiguration Branch
    {
        get => field ?? throw new InvalidOperationException("Branch configuration not set");
        private set;
    }

    public TagVersion BaseVersion
    {
        get => field ?? throw new InvalidOperationException("Base version not set");
        private set;
    }

    public int CommitHeight { get; private set; }

    public SemanticVersion? CalculatedVersion { get; set; }

    public VersionMetadata Metadata { get; } = new();

    public void SetBaseVersion(TagVersion version)
    {
        BaseVersion = version;
    }

    public void SetBranchConfiguration(VergilBranchConfiguration branchConfiguration)
    {
        Branch = branchConfiguration;
    }

    public void SetCommitHeight(int height)
    {
        CommitHeight = height;
    }
}
