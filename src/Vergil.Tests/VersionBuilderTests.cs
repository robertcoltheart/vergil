namespace Vergil.Tests;

public class VersionBuilderTests
{
    [Test]
    public async Task CanVersionCommitWithTag()
    {
        using var fixture = new RepositoryFixture();

        fixture.Commit();
        fixture.CommitAndTag("v1.0.0");
        fixture.CommitAndTag("v1.1.0");

        await fixture.AssertVersion("1.1.0");
    }

    [Test]
    public async Task CanVersionMainWithIncrementingMinor()
    {
        var configuration = new VergilConfiguration
        {
            Mode = IncrementMode.Continuous,
            Increment = VersionPart.Minor,
            Branhes =
            [
                new VergilBranchConfiguration
                {
                    Match = "^master$|^main$",
                    Label = string.Empty
                }
            ]
        };

        using var fixture = new RepositoryFixture(configuration);

        fixture.Commit();
        fixture.CommitAndTag("v1.0.0");
        fixture.CommitAndTag("v1.1.0");
        fixture.Commit();

        await fixture.AssertVersion("1.2.0");
    }


    [Test]
    public async Task CanVersionFeatureBranchFromTag()
    {
        var configuration = new VergilConfiguration
        {
            Mode = IncrementMode.Continuous,
            Increment = VersionPart.Minor,
            Branhes =
            [
                new VergilBranchConfiguration
                {
                    Match = "^master$|^main$",
                    Label = string.Empty
                },
                new VergilBranchConfiguration
                {
                    Match = @"^features?[\\/-](?<BranchName>.+)",
                    Mode = IncrementMode.Tagged,
                    Label = "${BranchName}"
                }
            ]
        };

        using var fixture = new RepositoryFixture(configuration);

        fixture.CommitAndTag("v1.0.0");
        fixture.Branch("feature/my-feature");

        await fixture.AssertVersion("1.1.0-my-feature.0");
    }
}
