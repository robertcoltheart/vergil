namespace Vergil;

public abstract class GitObject(ObjectId id)
{
    public ObjectId Id { get; } = id;
}
