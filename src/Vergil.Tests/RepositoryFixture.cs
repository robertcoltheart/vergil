using LibGit2Sharp;

namespace Vergil.Tests;

public class RepositoryFixture : IDisposable
{
    private readonly Action<VergilVersionBuilder>? action;

    private readonly string path = Path.Combine("Repositories", Guid.NewGuid().ToString("N"));

    private readonly IRepository repository;

    public RepositoryFixture(Action<VergilVersionBuilder>? action = null)
    {
        this.action = action;
        repository = new Repository(Repository.Init(path));
    }

    public void Checkout(string branch)
    {
        Commands.Checkout(repository, branch);
    }

    public string Commit()
    {
        var filename = Guid.NewGuid().ToString("N");

        var repositoryFilename = Path.Combine(repository.Info.WorkingDirectory, filename);

        File.WriteAllText(repositoryFilename, filename);

        Commands.Stage(repository, repositoryFilename);

        return repository.Commit("Changes", Signature(), Signature()).Sha;
    }

    public string CommitAndTag(string tag)
    {
        var sha = Commit();

        Tag(tag);

        return sha;
    }

    public void Branch(string branch)
    {
        Commands.Checkout(repository, repository.CreateBranch(branch));
    }

    public void Merge(string branch)
    {
        repository.Merge(repository.Branches[branch], Signature(), new MergeOptions
        {
            FastForwardStrategy = FastForwardStrategy.NoFastForward
        });
    }

    public void Tag(string tag)
    {
        repository.ApplyTag(tag);
    }

    public async Task AssertVersion(string version)
    {
        var builder = new VergilVersionBuilder()
            .ForPath(path);

        action?.Invoke(builder);

        var calculated = builder.Build();

        await Assert.That(calculated.SemanticVersion).IsEqualTo(version);
    }

    public void Dispose()
    {
        repository.Dispose();

        try
        {
            Directory.Delete(path, true);
        }
        catch
        {
            // Ignored
        }
    }

    private Signature Signature()
    {
        return new Signature("Committer", "test@host.com", DateTime.UtcNow);
    }
}
