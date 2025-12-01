namespace Vergil;

public class DirectReference : Reference
{
    public override DirectReference ResolveDirectReference()
    {
        return this;
    }
}
