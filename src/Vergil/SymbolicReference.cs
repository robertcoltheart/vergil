namespace Vergil;

public class SymbolicReference(string canonicalName, string targetIdentifier, Reference reference) : Reference
{
    public Reference Target => reference;

    public override DirectReference? ResolveDirectReference()
    {
        return reference.ResolveDirectReference();
    }
}
