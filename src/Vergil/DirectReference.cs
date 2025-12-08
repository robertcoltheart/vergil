namespace Vergil;

public class DirectReference(IRepository repository, string canonicalName, ObjectId targetId) : Reference
{
    public GitObject? Target => repository.Lookup(targetId);

    public override DirectReference ResolveDirectReference()
    {
        return this;
    }
}
