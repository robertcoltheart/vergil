using System.Collections;

namespace Vergil;

public class ReferenceCollection(IRepository repository) : IEnumerable<Reference>
{
    private const string SymbolicRef = "ref: ";

    private static readonly string[] Prefixes =
    [
        string.Empty,
        "refs/",
        "refs/tags/",
        "refs/heads/",
        "refs/remotes/"
    ];

    public Reference? this[string name] => Resolve(name);

    public Reference? Head => Resolve("HEAD");

    public IEnumerator<Reference> GetEnumerator()
    {
        var path = Path.Combine(repository.Info.Path, "refs");

        var references = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            .Select(x => x.Substring(path.Length + 1))
            .Select(Resolve);

        return references.GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Reference? Resolve(string name)
    {
        return ResolveLoose(name) ?? ResolvePacked(name);
    }

    private Reference? ResolveLoose(string name)
    {
        var path = Path.Combine(repository.Info.Path, name);

        if (!File.Exists(path))
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

            return new SymbolicReference(name, targetName, target);
        }

        var id = ObjectId.Parse(data);

        return new DirectReference(repository, name, id);
    }

    private Reference? ResolvePacked(string name)
    {
        var path = Path.Combine(repository.Info.Path, "packed-refs");

        if (File.Exists(path))
        {
            using var reader = new StreamReader(File.OpenRead(path));

            var line = reader.ReadLine();

            while (line != null)
            {
                if (!string.IsNullOrEmpty(line) && line[0] != (byte) '#')
                {
                    var id = line[..40];
                    var refName = line.Substring(id.Length + 1);

                    if (refName == name)
                    {
                        return new DirectReference(repository, "", default);
                    }
                }

                line = reader.ReadLine();
            }
        }

        return null;
    }
}
