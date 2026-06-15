using System.Text.RegularExpressions;

namespace Vergil.Tests;

public class VergilConfigurationTests
{
    [Test]
    [Arguments("v|V?", "v1.0.0", "1.0.0")]
    [Arguments("^v|V?", "v1.0.0", "1.0.0")]
    [Arguments("v|V?", "v1.0.0v", "1.0.0v")]
    [Arguments("v|V?", "V1.0.0", "1.0.0")]
    [Arguments("^v|V?", "V1.0.0", "1.0.0")]
    [Arguments("v|V?", "V1.0.0V", "1.0.0V")]
    [Arguments("v|V?", "1.0.0", "1.0.0")]
    [Arguments("^v|V?", "1.0.0", "1.0.0")]
    public async Task CanParseTagPrefix(string prefix, string input, string expected)
    {
        prefix = $"^({prefix})";

        var actual = Regex.Replace(input, prefix, string.Empty);

        await Assert.That(actual).IsEqualTo(expected);
    }
}
