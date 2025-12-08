namespace Vergil;

public class Branch(IRepository repository, Reference reference) : ReferencePointer<Commit>(repository, reference)
{
    public Commit? Tip => TargetObject;
}
