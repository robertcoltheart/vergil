namespace Vergil.Tests;

/// <summary>
/// Library scenarios:
///   - Current commit has tag (1.4.0)
///   - On master, with height (1.4.0+3)
///   - On branch, even with master (1.4.0-branch-name.0)
///   - On branch, with height (1.4.0-branch-name.3)
/// App scenarios:
///   - Current commit has tag (1.4.0)
///   - On master, with height (1.7.0)
///   - On branch, even with master (1.4.0-branch-name.0)
///   - On branch, with height (1.4.0-branch-name.3)
///   - On release branch, even with master (1.4.0)
///   - On release branch, with height (1.4.3)
/// </summary>
public class VersionBuilderTests
{
    [Test]
    public async Task CanVersionCommitWithTag()
    {
        using var fixture = new RepositoryFixture();

        fixture.CommitAndTag("v1.1.0");

        await fixture.AssertVersion("1.1.0");
    }

    [Test]
    public async Task CanVersionCommitWithTagAndCustomLabel()
    {
        using var fixture = new RepositoryFixture(x => x.WithLabel("alpha"));

        fixture.CommitAndTag("v1.1.0");

        await fixture.AssertVersion("1.1.0");
    }

    [Test]
    public async Task CanVersionCommitWithMultipleTags()
    {
        using var fixture = new RepositoryFixture();

        fixture.Commit();
        fixture.Tag("v1.1.0");
        fixture.Tag("v1.1.2");

        await fixture.AssertVersion("1.1.2");
    }

    [Test]
    public async Task CanVersionCommitWithMultipleTagsAndCustomLabel()
    {
        using var fixture = new RepositoryFixture(x => x.WithLabel("alpha"));

        fixture.Commit();
        fixture.Tag("v1.1.0");
        fixture.Tag("v1.1.2");

        await fixture.AssertVersion("1.1.2");
    }

    [Test]
    public async Task CanVersionCommitWithTagAndHeight()
    {
        using var fixture = new RepositoryFixture();

        fixture.CommitAndTag("v1.1.0");
        fixture.Commit();

        await fixture.AssertVersion("1.1.0");
    }
}
