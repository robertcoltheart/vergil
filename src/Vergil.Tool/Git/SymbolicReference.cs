namespace Vergil.Tool.Git;

public class SymbolicReference(string canonicalName, string targetIdentifier, Reference? target) : Reference(canonicalName, targetIdentifier)
{
    public Reference? Target { get; } = target;

    public override DirectReference? ResolveToDirectReference()
    {
        return Target?.ResolveToDirectReference();
    }
}
