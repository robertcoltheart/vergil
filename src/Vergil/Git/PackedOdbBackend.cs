namespace Vergil.Git;

public class PackedOdbBackend(string path) : OdbBackend
{
    private readonly string PackPath = Path.Combine(path, "objects", "pack");

    private readonly List<PackFile> packFiles = [];

    private bool initialized;

    public override GitObjectEntry? Read(ObjectId id)
    {
        EnsurePackFiles();

        return packFiles
            .Select(x => x.Read(id))
            .FirstOrDefault(x => x != null);
    }

    public override void Dispose()
    {
        foreach (var packFile in packFiles)
        {
            packFile.Dispose();
        }
    }

    private void EnsurePackFiles()
    {
        if (initialized)
        {
            return;
        }

        if (Directory.Exists(PackPath))
        {
            var packs = Directory.GetFiles(PackPath, "*.idx")
                .Select(x => new { IndexFile = x, PackFile = Path.ChangeExtension(x, "pack") })
                .Where(x => File.Exists(x.PackFile));

            foreach (var pack in packs)
            {
                var index = new PackIndex(pack.IndexFile);
                var file = new PackFile(pack.PackFile, index);

                packFiles.Add(file);
            }
        }

        initialized = true;
    }
}
