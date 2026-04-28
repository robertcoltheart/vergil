namespace Vergil.Git;

public class TagAnnotation(IRepository repository, ObjectId id, ObjectId targetId) : GitObject(id)
{
    private const int ObjectEntryLength = 48;

    private static readonly byte[] Object = "object "u8.ToArray();

    private readonly Lazy<GitObject?> targetResolver = new(() => repository.Lookup(targetId));

    public GitObject Target => targetResolver.Value ?? throw new InvalidOperationException("Target object not found");

    public static TagAnnotation Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data)
    {
        var parsed = new ParsedTag();

        var buffer = data.Slice(ObjectEntryLength);

        if (buffer.Slice(0, Object.Length).SequenceEqual(Object))
        {
            var target = buffer.Slice(Object.Length, 40);

            parsed.ObjectId = new ObjectId(target);
        }

        if (parsed.ObjectId == null)
        {
            throw new InvalidOperationException("Object field invalid");
        }

        return new TagAnnotation(repository, id, parsed.ObjectId);
    }

    private record struct ParsedTag(ObjectId? ObjectId);
}
