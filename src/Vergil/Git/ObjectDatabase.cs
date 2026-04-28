namespace Vergil.Git;

public class ObjectDatabase(IRepository repository, params OdbBackend[] backends) : IDisposable
{
    private readonly Dictionary<ObjectId, GitObject?> cache = new();

    public GitObject? Read(ObjectId id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }

        var contents = backends
            .Select(x => x.Read(id))
            .FirstOrDefault(x => x != null);

        var parsed = ParseGitObject(id, contents);

        cache[id] = parsed;

        return parsed;
    }

    public void Dispose()
    {
        foreach (var backend in backends)
        {
            backend.Dispose();
        }
    }

    private GitObject? ParseGitObject(ObjectId id, GitObjectEntry? entry)
    {
        if (entry == null)
        {
            return null;
        }

        var payload = entry.Value;

        return GitObject.Parse(repository, id, payload.Data.Span, payload.Type);
    }
}
