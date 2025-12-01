namespace Vergil.IO;

public interface IFileSystem
{
    RawObject? Read(ObjectId id);
}
