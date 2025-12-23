using Vergil.Simple;

namespace Vergil.Tests;

public class SemanticVersionTests
{
    [Test]
    [Arguments("1.0.0", "1.0.0")]
    [Arguments("1.5.6-beta.1", "1.5.6-beta.1")]
    [Arguments("2.5.4-beta.1+info.version", "2.5.4-beta.1+info.version")]
    [Arguments("1.0.0-beta", "1.0.0-BETA")]
    [Arguments("1.0.0-beta+info", "1.0.0-beta+INFO")]
    [Arguments("1.0.0", "1.0.0+info")]
    public async Task VersionsAreEqual(string x, string y)
    {
        var xVersion = SemanticVersion.Parse(x);
        var yVersion = SemanticVersion.Parse(y);

        await Assert.That(xVersion).IsEqualTo(yVersion);
    }

    [Test]
    [Arguments("0.0.0", "1.0.0")]
    [Arguments("1.0.0", "1.0.1")]
    [Arguments("1.0.0", "1.1.0")]
    [Arguments("1.99.99", "1.0.0-beta2")]
    [Arguments("1.0.0-beta", "1.0.0-beta2")]
    [Arguments("1.0.0-beta", "1.0.0")]
    [Arguments("1.0.0-beta", "1.0.0-beta.1")]
    [Arguments("1.0.0-beta.1", "1.0.0-beta.2")]
    [Arguments("1.0.0-beta.3.56", "1.0.0-beta.3.57")]
    [Arguments("1.0.0-beta.3.56+abc", "1.0.0-beta.3.57+def")]
    public async Task VersionIsLessThanOther(string x, string y)
    {
        var xVersion = SemanticVersion.Parse(x);
        var yVersion = SemanticVersion.Parse(y);

        await Assert.That(xVersion).IsEqualTo(yVersion);
    }
}
