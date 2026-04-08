namespace Vergil.Simple;

public class ObjectDirectory(string directory)
{
    private const uint TableOfContents = 0xff744f63;

    private const uint Version = 2;

    private readonly PackIndex[] indices = ScanPackFiles(directory).ToArray();

    public Commit ReadCommit(string sha)
    {
        return ReadCommitPacked(sha) ?? ReadCommitLoose(sha) ?? throw new InvalidOperationException("Sha not found");
    }

    private Commit? ReadCommitPacked(string sha)
    {
        var initial = Convert.ToByte(sha[..2], 16);

        return null;
    }

    private Commit? ReadCommitLoose(string sha)
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

    private static IEnumerable<PackIndex> ScanPackFiles(string directory)
    {
        var files = Directory.GetFiles(Path.Combine(directory, "objects", "pack"), "*.idx");

        foreach (var file in files)
        {
            using var stream = File.OpenRead(file);
            using var reader = new BinaryReader(stream);

            var header = reader.ReadUInt32BigEndian();
            var version = reader.ReadUInt32BigEndian();

            if (header != TableOfContents)
            {
                throw new InvalidOperationException("Bad index file");
            }

            if (version != Version)
            {
                throw new InvalidOperationException("Bad index file");
            }

            yield return new PackIndex(stream);
        }
    }
}
