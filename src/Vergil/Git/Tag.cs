namespace Vergil.Git;

public class Tag(IRepository repository, Reference reference, string canonicalName) : ReferenceWrapper<GitObject>(repository, reference, canonicalName)
{
    public TagAnnotation? Annotation => TargetObject as TagAnnotation;

    public bool IsAnnotated => Annotation != null;

    public GitObject? Target => Annotation == null
        ? TargetObject
        : Annotation.Target;

    public GitObject? PeeledTarget
    {
        get
        {
            var target = TargetObject;

            while (target is TagAnnotation annotation)
            {
                target = annotation.Target;
            }

            return target;
        }
    }

    protected override string Shorten()
    {
        return CanonicalName.Substring(Reference.TagPrefix.Length);
    }
}
