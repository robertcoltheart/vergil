namespace Vergil.Simple;

public class Repository(string directory)
{
    private readonly References references = new(directory);

    private readonly ObjectDirectory objects = new(directory);

    public Commit GetHead()
    {
        var sha = references.Resolve("HEAD");

        if (string.IsNullOrEmpty(sha))
        {
            throw new InvalidOperationException("HEAD missing from repository");
        }

        return Lookup(sha);
    }

    public IEnumerable<Tag> GetTags()
    {
        return references.GetTags();
    }

    public Commit Lookup(string sha)
    {
        return objects.ReadCommit(sha);
    }
}
