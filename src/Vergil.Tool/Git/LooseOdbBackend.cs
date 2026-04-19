using System.IO.Compression;

namespace Vergil.Tool.Git;

public class LooseOdbBackend(string path) : OdbBackend
{
    private readonly string objectsPath = Path.Combine(path, "objects");

    public override bool Exists(ObjectId id)
    {
        var path = GetPath(id.Sha);

        return File.Exists(path);
    }

    public override bool Exists(string shortSha)
    {
        if (shortSha.Length < 2)
        {
            throw new ArgumentException("Incomplete short id");
        }

        var path = Path.Combine(objectsPath, shortSha[..2]);

        if (!Directory.Exists(path))
        {
            return false;
        }

        var prefix = shortSha[2..];

        var files = Directory.GetFiles(path)
            .Select(x => Path.GetFileName(x))
            .Where(x => string.IsNullOrEmpty(prefix) || x.StartsWith(prefix))
            .ToArray();

        return files.Length == 1;
    }

    public override byte[]? Read(ObjectId id)
    {
        var path = GetPath(id.Sha);

        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var deflate = new DeflateStream(stream, CompressionMode.Decompress);
        using var memory = new MemoryStream();

        stream.Seek(2, SeekOrigin.Begin);
        deflate.CopyTo(memory);

        return memory.ToArray();
    }

    public override byte[]? Read(string shortSha)
    {
        throw new NotImplementedException();
    }

    private string GetPath(string sha)
    {
        return Path.Combine(objectsPath, sha[..2], sha[2..]);
    }
}
