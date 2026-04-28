using System.Collections.Frozen;
using System.Text;

namespace Vergil.Git;

public class LooseOdbBackend(string path) : OdbBackend
{
    private readonly string objectsPath = Path.Combine(path, "objects");

    private readonly FrozenSet<string> initialPaths = GetInitialPaths(path).ToFrozenSet();

    public override GitObjectEntry? Read(ObjectId id)
    {
        var path = GetPath(id.Sha);

        if (!initialPaths.Contains(id.Sha[..2]))
        {
            return null;
        }

        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var deflate = new ZLibStream(stream);

        var header = ParseHeader(deflate);

        var buffer = new byte[header.Length];
        deflate.ReadExactly(buffer);

        return new GitObjectEntry(header.Type, buffer);
    }

    private string GetPath(string sha)
    {
        return Path.Combine(objectsPath, sha[..2], sha[2..]);
    }

    private ObjectHeader ParseHeader(Stream stream)
    {
        var value = new StringBuilder();

        var b = stream.ReadByte();

        while (b != 0)
        {
            value.Append(Convert.ToChar(b));

            b = stream.ReadByte();
        }

        var header = value.ToString();

        if (header.Split(' ') is not [var type, var length])
        {
            throw new InvalidOperationException("Invalid object header");
        }

        return new ObjectHeader(ParseObjectType(type), int.Parse(length));
    }

    private ObjectType ParseObjectType(string header)
    {
        return header switch
        {
            "commit" => ObjectType.Commit,
            "tree" => ObjectType.Tree,
            "blob" => ObjectType.Blob,
            "tag" => ObjectType.Tag,
            _ => throw new ArgumentException("Unknown object type", nameof(header))
        };
    }

    private static HashSet<string> GetInitialPaths(string path)
    {
        return Directory.GetDirectories(Path.Combine(path, "objects"))
            .Where(x => x.Length == 2)
            .ToHashSet();
    }

    private record struct ObjectHeader(ObjectType Type, int Length);
}
