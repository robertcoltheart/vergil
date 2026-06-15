using System.Collections;

namespace Vergil.Git;

public class TagCollection(IRepository repository) : IEnumerable<Tag>
{
    public Tag? this[string name] => Resolve(name);

    public IEnumerator<Tag> GetEnumerator()
    {
        return repository.Refs
            .Where(x => x.IsTag)
            .Select(x => Resolve(x.CanonicalName))
            .Where(x => x != null)
            .GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Tag? Resolve(string name)
    {
        var canonicalName = NormalizeToCanonicalName(name);
        var reference = repository.Refs.Resolve(canonicalName);

        if (reference == null)
        {
            return null;
        }

        return new Tag(repository, reference, canonicalName);
    }

    private string NormalizeToCanonicalName(string name)
    {
        if (name.LooksLikeTag())
        {
            return name;
        }

        return $"{Reference.TagPrefix}{name}";
    }
}
