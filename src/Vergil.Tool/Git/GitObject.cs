namespace Vergil.Tool.Git;

public abstract class GitObject(ObjectId id) : IEquatable<GitObject>
{
    public ObjectId Id { get; } = id;

    public string Sha => Id.Sha;

    public static GitObject Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data, ObjectType type)
    {
        return type switch
        {
            ObjectType.Commit => Commit.Parse(repository, id, data),
            ObjectType.Tag => TagAnnotation.Parse(repository, id, data),
            _ => throw new InvalidOperationException($"Git object type not supported: {type}")
        };
    }

    public bool Equals(GitObject? other)
    {
        return other?.Id.Equals(Id) == true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GitObject);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}
