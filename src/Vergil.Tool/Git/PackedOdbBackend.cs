namespace Vergil.Tool.Git;

public class PackedOdbBackend(string path) : OdbBackend
{
    private readonly string PackPath = Path.Combine(path, "objects", "pack");

    private readonly List<PackFile> files = [];

    private bool initialized;

    public override bool Exists(ObjectId id)
    {
        EnsurePackFiles();

        throw new NotImplementedException();
    }

    public override bool Exists(string shortSha)
    {
        EnsurePackFiles();

        throw new NotImplementedException();
    }

    public override byte[]? Read(ObjectId id)
    {
        EnsurePackFiles();

        foreach (var file in files)
        {
            var offset = file.GetOffset(id);

            if (offset != null)
            {
                return null;
            }
        }

        return null;
    }

    public override byte[]? Read(string shortSha)
    {
        EnsurePackFiles();

        throw new NotImplementedException();
    }

    private void EnsurePackFiles()
    {
        if (initialized)
        {
            return;
        }

        if (Directory.Exists(PackPath))
        {
            var indexFiles = Directory.GetFiles(PackPath, "*.idx");

            foreach (var indexFile in indexFiles)
            {
                files.Add(new PackFile(indexFile));
            }
        }

        initialized = true;
    }
}
