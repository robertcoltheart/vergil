using Vergil.Simple;

var repository = new Repository(@"C:\Projects\machine\machine.specifications\.git");
var tags = repository.GetTags();

var tagHashes = tags.Select(x => x.Sha).ToHashSet();

var head = repository.GetHead();

var check = new Stack<Commit>();
check.Push(head);

while (check.TryPop(out var commit))
{
    Console.WriteLine($"Commit: {commit.ShortSha}");

    if (tagHashes.Contains(commit.Sha))
    {
        break;
    }

    foreach (var parent in commit.Parents)
    {
        var parentCommit = repository.Lookup(parent);

        check.Push(parentCommit);
    }
}

Console.WriteLine("Hello, World!");
