using System.Collections;
using System.Text;

namespace Vergil.Tool.Git;

public class ObjectDatabase(IRepository repository, params OdbBackend[] backends) : IEnumerable<GitObject>, IDisposable
{
    private const byte HeaderSeparator = 0;

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

    public GitObject? Read(string objectish)
    {
        return null;
    }

    public IEnumerator<GitObject> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        foreach (var backend in backends)
        {
            backend.Dispose();
        }
    }

    private GitObject? ParseGitObject(ObjectId id, ReadOnlySpan<byte> payload)
    {
        if (payload.Length == 0)
        {
            return null;
        }

        var separator = payload.IndexOf(HeaderSeparator);

        var header = payload[..separator];
        var data = payload[(separator + 1)..];

        var objectType = ParseObjectType(Encoding.UTF8.GetString(header));

        return GitObject.Parse(repository, id, data, objectType);
    }

    private ObjectType ParseObjectType(string header)
    {
        return header switch
        {
            _ when header.StartsWith("commit ") => ObjectType.Commit,
            _ when header.StartsWith("tree ") => ObjectType.Tree,
            _ when header.StartsWith("blob ") => ObjectType.Blob,
            _ when header.StartsWith("tag ") => ObjectType.Tag,
            _ => throw new ArgumentException("Unknown object type", nameof(header))
        };
    }
}
