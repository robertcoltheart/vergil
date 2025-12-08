using Vergil.Simple;

namespace Vergil.Tests;

public class GitTests
{
    [Test]
    public async Task Test()
    {
        var repository = new Repository(@"C:\Projects\machine\machine.specifications\.git");

        var head = repository.GetHead();
        var tags = repository.GetTags().ToArray();

        await Assert.That(true).IsTrue();
    }
}
