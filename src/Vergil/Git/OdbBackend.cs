namespace Vergil.Git;

public abstract class OdbBackend : IDisposable
{
    public abstract GitObjectEntry? Read(ObjectId id);

    public virtual void Dispose()
    {
    }
}
