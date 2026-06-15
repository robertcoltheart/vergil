namespace Vergil.Git;

public class DetachedHead(IRepository repository, Reference reference) : Branch(repository, reference)
{
    protected override string Shorten()
    {
        return CanonicalName;
    }
}
