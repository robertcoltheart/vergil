using System.Xml.Linq;

namespace Vergil.Simple;

public class References(string directory)
{
    private const string SymbolicRef = "ref: ";

    private readonly Dictionary<string, string> packed = LoadPackedReferences(directory);

    public IEnumerable<Tag> GetTags()
    {
        return GetLooseTags().Concat(GetPackedTags());
    }

    public string? Resolve(string name)
    {
        return ResolveLoose(name) ?? ResolvePacked(name);
    }

    private IEnumerable<Tag> GetLooseTags()
    {
        var path = Path.Combine(directory, "refs", "tags");
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var sha = File.ReadAllText(file).TrimEnd();

            yield return new Tag(name, sha);
        }
    }

    private IEnumerable<Tag> GetPackedTags()
    {

    }

    private string? ResolveLoose(string name)
    {
        var paths = new[]
        {
            Path.Combine(directory, name),
            Path.Combine(directory, "refs", "heads", name),
            Path.Combine(directory, "refs", "tags", name),
            Path.Combine(directory, "refs", "remotes", name),
        };

        var path = paths.FirstOrDefault(File.Exists);

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var data = File.ReadAllText(path).TrimEnd();

        if (data.StartsWith(SymbolicRef))
        {
            var targetName = data[SymbolicRef.Length..];
            var target = Resolve(targetName);

            if (target == null)
            {
                return null;
            }

            return target;
        }

        return data;
    }

    private string? ResolvePacked(string name)
    {
    }

    private static Dictionary<string, string> LoadPackedReferences(string directory)
    {
        var path = Path.Combine(directory, "packed-refs");

        var references = new Dictionary<string, string>();

        if (File.Exists(path))
        {
            using var reader = new StringReader(File.ReadAllText(path));

            var line = reader.ReadLine();

            var isPeeled = line?.StartsWith("# pack-refs with:") == true && line.Contains("peeled");
            var last = default(PackedReference);

            while (line != null)
            {
                if (line.StartsWith('#'))
                {
                    continue;
                }

                if (line.StartsWith('^'))
                {
                    if (last == null)
                    {
                        throw new InvalidOperationException("Missing target for peeled reference");
                    }

                    references[line[1..]] = last.Reference;
                }

                var id = line[..40];
                var reference = line[(id.Length + 1)..];

                last = new PackedReference(id, reference);

                references[id] = reference;

                line = reader.ReadLine();
            }
        }

        return references;
    }

    private class PackedReference(string id, string reference)
    {
        public string Id { get; } = id;

        public string Reference { get; } = reference;
    }
}
