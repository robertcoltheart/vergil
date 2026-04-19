namespace Vergil.Tool.Git;

public abstract class OdbBackend : IDisposable
{
    public abstract bool Exists(ObjectId id);

    public abstract bool Exists(string shortSha);

    public abstract byte[]? Read(ObjectId id);

    public abstract byte[]? Read(string shortSha);

    public virtual void Dispose()
    {
    }
}
