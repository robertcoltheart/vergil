using LibGit2Sharp;

namespace Vergil.Tests;

public class LibGitTests
{
    [Test]
    public void Test()
    {
        var repository = new Repository(@"C:\Projects\machine\machine.specifications\.git");

        var tags = repository.Tags.ToArray();
    }
}
