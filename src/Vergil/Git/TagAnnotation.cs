using System.Text;

namespace Vergil.Git;

public class TagAnnotation(IRepository repository, ObjectId id, ObjectId targetId) : GitObject(id)
{
    private readonly Lazy<GitObject?> targetResolver = new(() => repository.Lookup(targetId));

    public GitObject Target => targetResolver.Value ?? throw new InvalidOperationException("Target object not found");

    public static TagAnnotation Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data)
    {
        using var reader = new StringReader(Encoding.UTF8.GetString(data));

        var parsed = new ParsedTag();

        var line = reader.ReadLine();

        while (line != null)
        {
            if (line.Length == 0)
            {
                break;
            }

            if (line.StartsWith("object "))
            {
                parsed.ObjectId = new ObjectId(line[7..]);
            }

            line = reader.ReadLine();
        }

        if (parsed.ObjectId == null)
        {
            throw new InvalidOperationException("Object field invalid");
        }

        return new TagAnnotation(repository, id, parsed.ObjectId);
    }

    private record struct ParsedTag(ObjectId? ObjectId);
}
