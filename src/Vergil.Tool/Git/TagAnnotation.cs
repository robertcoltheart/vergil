using System.Text;

namespace Vergil.Tool.Git;

public class TagAnnotation(IRepository repository, ObjectId id, string name, string message) : GitObject(id)
{
    public string Name { get; } = name;

    public string Message { get; } =  message;

    public GitObject Target { get; }

    public Signature Tagger { get; }

    public static TagAnnotation Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data)
    {
        var value = Encoding.UTF8.GetString(data);

        return new TagAnnotation(repository, id, "", "");
    }
}
