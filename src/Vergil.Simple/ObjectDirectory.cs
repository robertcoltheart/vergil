namespace Vergil.Simple;

public class ObjectDirectory(string directory)
{
    private const uint TableOfContents = 0xff744f63;

    private const uint Version = 2;

    private readonly PackIndex[] indices = ScanPackFiles(directory).ToArray();

    public Commit ReadCommit(string sha)
    {
        return null;
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
