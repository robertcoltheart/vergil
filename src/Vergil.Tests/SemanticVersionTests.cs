using Vergil.Simple;

namespace Vergil.Tests;

public class SemanticVersionTests
{
    [Test]
    [Arguments("0.0.0")]
    [Arguments("1.0.0")]
    [Arguments("1.1.0")]
    [Arguments("1.1.0-beta1")]
    [Arguments("1.1.0-beta.1")]
    [Arguments("1.1.0-beta.1+info")]
    [Arguments("1.1.0-beta.1+info.version")]
    [Arguments("1.1.0-beta.1+info.version.01")]
    public async Task CanParseVersion(string value)
    {
        var version = SemanticVersion.Parse(value);

        await Assert.That(version).IsNotNull();
        await Assert.That(version.ToString()).IsEqualTo(value);
    }

    [Test]
    [Arguments("1.0")]
    [Arguments("1.0.0.0")]
    [Arguments("1.0-beta")]
    [Arguments("1.0 .0")]
    [Arguments("1.0.0.0-beta")]
    [Arguments("1.0.0-beta..1")]
    [Arguments("01.0.0")]
    [Arguments("1.01.0")]
    [Arguments("1.0.01")]
    [Arguments(".1.0")]
    [Arguments("1.1.")]
    [Arguments("1.0.0-a*c")]
    [Arguments("a.b.c")]
    [Arguments("1.0.0-00")]
    [Arguments("1.0.0-beta.00.a")]
    public async Task InvalidVersionsAreNotParsed(string value)
    {
        var parsed = SemanticVersion.TryParse(value, out _);

        await Assert.That(parsed).IsFalse();
    }

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
    [Arguments("1.99.99", "2.0.0-beta2")]
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

        await Assert.That(xVersion).IsLessThan(yVersion);
    }
}
