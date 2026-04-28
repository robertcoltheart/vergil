namespace Vergil.Git;

public abstract class ReferenceWrapper<T>(IRepository repository, Reference reference, string canonicalName) : IEquatable<ReferenceWrapper<T>>
    where T : GitObject
{
    private readonly Lazy<T?> targetObjectResolver = new(() => GetTargetObject(repository, reference));

    protected T? TargetObject => targetObjectResolver.Value;

    public Reference Reference { get; } = reference;

    public string CanonicalName { get; } = canonicalName;

    public string FriendlyName => Shorten();

    protected abstract string Shorten();

    public bool Equals(ReferenceWrapper<T>? other)
    {
        return other?.CanonicalName == CanonicalName && other.Reference.TargetIdentifier == Reference.TargetIdentifier;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReferenceWrapper<T>);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CanonicalName, Reference.TargetIdentifier);
    }

    public override string ToString()
    {
        var target = TargetObject != null
            ? TargetObject.Id.ToString(7)
            : "?";

        return $"{CanonicalName} => \"{target}\"";
    }

    private static T? GetTargetObject(IRepository repository, Reference reference)
    {
        var directReference = reference.ResolveToDirectReference();

        var target = directReference?.Target;

        if (target == null)
        {
            return null;
        }

        return repository.Lookup<T>(target.Id);
    }
}
