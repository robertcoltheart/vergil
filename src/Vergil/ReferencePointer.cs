namespace Vergil;

public abstract class ReferencePointer<T>(IRepository repository, Reference reference)
    where T : GitObject
{
    protected T? TargetObject => field ??= GetTargetObject();

    private T? GetTargetObject()
    {
        var directReference = reference.ResolveDirectReference();

        if (directReference == null)
        {
            return null;
        }

        var target = directReference.Target;

        if (target == null)
        {
            return null;
        }

        return repository.Lookup<T>(target.Id);
    }
}
