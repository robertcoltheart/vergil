using System.IO.Compression;

namespace Vergil.Simple;

public class Repository(string directory)
{
    private const string SymbolicRef = "ref: ";

    public Commit GetHead()
    {
        var sha = Resolve("HEAD");

        if (string.IsNullOrEmpty(sha))
        {
            throw new InvalidOperationException("HEAD missing from repository");
        }

        return ReadCommit(sha);
    }

    public IEnumerable<Tag> GetTags()
    {
        var path = Path.Combine(directory, "refs", "tags");
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var sha = File.ReadAllText(file).TrimEnd();

            yield return new Tag(name, sha);
        }
    }

    private Commit ReadCommit(string sha)
    {
        var path = Path.Combine(directory, "objects", sha[..2], sha[2..]);

        if (!File.Exists(path))
        {
            throw new InvalidOperationException("Object database corrupted");
        }

        using var stream = File.OpenRead(path);
        using var deflate = new DeflateStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(deflate);

        stream.Seek(2, SeekOrigin.Begin);

        var data = reader.ReadToEnd();

        return new Commit("");
    }

    private string? Resolve(string name)
    {
        return ResolveLoose(name);
    }

    private string? ResolveLoose(string name)
    {
        var path = Path.Combine(directory, name);

        if (!File.Exists(path))
        {
            return null;
        }

        var data = File.ReadAllText(path).TrimEnd();

        if (data.StartsWith(SymbolicRef))
        {
            var targetName = data[SymbolicRef.Length..];
            var target = Resolve(targetName);

            if (target == null)
            {
                return null;
            }

            return target;
        }

        return data;
    }
}
