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

        return ReadCommit(sha);
    }

    public IEnumerable<Tag> GetTags()
    {
        return references.GetTags();
    }

    public Commit Lookup(string sha)
    {
        return ReadCommit(sha) ?? objects.ReadCommit(sha);
    }

    private Commit? ReadCommit(string sha)
    {
        var path = Path.Combine(directory, "objects", sha[..2], sha[2..]);

        if (!File.Exists(path))
        {
            return null;
        }

        using var reader = new ObjectReader(path);

        var commit = new Commit(sha);

        var (type, reference) = reader.Read();

        while (!string.IsNullOrEmpty(type))
        {
            if (type == "parent")
            {
                commit.Parents.Add(reference);
            }

            (type, reference) = reader.Read();
        }

        return commit;
    }
}
