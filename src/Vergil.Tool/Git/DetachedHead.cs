namespace Vergil.Tool.Git;

public class DetachedHead(IRepository repository, Reference reference) : Branch(repository, reference)
{
    public override Branch? TrackedBranch => null;

    protected override string Shorten()
    {
        return CanonicalName;
    }
}
