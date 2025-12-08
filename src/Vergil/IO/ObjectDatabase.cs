namespace Vergil.IO;

public class ObjectDatabase(IRepository repository) : IObjectDatabase
{
    private readonly IFileSystem[] fileSystems =
    [
        new PackedFileSystem(),
        new LooseFileSystem(repository)
    ];

    public RawObject? Read(ObjectId id)
    {
        throw new NotImplementedException();
    }
}
