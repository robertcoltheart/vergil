using System.Collections;

namespace Vergil.Tool.Git;

public class TagCollection(IRepository repository) : IEnumerable<Tag>
{
    public Tag? this[string name]
    {
        get
        {
            var canonicalName = NormalizeToCanonicalName(name);
            var reference = repository.Refs.Resolve(canonicalName);

            if (reference == null)
            {
                return null;
            }

            return new Tag(repository, reference, canonicalName);
        }
    }

    public IEnumerator<Tag> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
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
