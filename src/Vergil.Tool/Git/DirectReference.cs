namespace Vergil.Tool.Git;

public class DirectReference(IRepository repository, string canonicalName, ObjectId id) : Reference(canonicalName, id.Sha)
{
    private readonly Lazy<GitObject?> targetResolver = new(() => repository.Lookup(id));

    public GitObject? Target => targetResolver.Value;

    public override DirectReference? ResolveToDirectReference()
    {
        return this;
    }
}
