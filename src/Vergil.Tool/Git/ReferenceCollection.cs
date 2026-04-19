using System.Collections;

namespace Vergil.Tool.Git;

public class ReferenceCollection(IRepository repository) : IEnumerable<Reference>
{
    private const string SymbolicRef = "ref: ";

    private readonly string[] referencePaths =
    [
        Path.Combine(repository.Info.Path, "refs", "heads"),
        Path.Combine(repository.Info.Path, "refs", "tags"),
        Path.Combine(repository.Info.Path, "refs", "remotes")
    ];

    private readonly string packedReferences = Path.Combine(repository.Info.Path, "packed-refs");

    public Reference? this[string name] => Resolve(name);

    public Reference? Head => this["HEAD"];

    public Reference? Resolve(string name)
    {
        return ResolveLooseReference(name) ?? ResolvePackedReference(name);
    }

    public IEnumerator<Reference> GetEnumerator()
    {
        return GetLooseReferences()
            .Concat(GetPackedReferences())
            .Select(Resolve)
            .Where(x => x != null)
            .GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerable<string> GetLooseReferences()
    {
        return referencePaths
            .SelectMany(x => Directory.GetFiles(x, "*", SearchOption.AllDirectories))
            .Select(x => Path.GetFileName(x));
    }

    private Reference? ResolveLooseReference(string name)
    {
        var reference = ParseReference(repository.Info.Path, name);

        if (reference != null)
        {
            return reference;
        }

        return referencePaths
            .Select(x => ParseReference(x, name))
            .FirstOrDefault();
    }

    private Reference? ParseReference(string basePath, string name)
    {
        var referencePath = Path.Combine(basePath, name);

        if (!File.Exists(referencePath))
        {
            return null;
        }

        var contents = File.ReadAllText(referencePath).TrimEnd();

        if (contents.StartsWith(SymbolicRef))
        {
            var targetIdentifier = contents[SymbolicRef.Length..];
            var target = Resolve(targetIdentifier);

            if (target == null)
            {
                return null;
            }

            return new SymbolicReference(name, targetIdentifier, target);
        }

        return new DirectReference(repository, name, new ObjectId(contents));
    }

    private IEnumerable<string> GetPackedReferences()
    {
        if (!File.Exists(packedReferences))
        {
            return [];
        }

        return File.ReadAllLines(packedReferences)
            .Where(x => !x.StartsWith('#') && !x.StartsWith('^'))
            .Select(x => x.Split(' '))
            .Where(x => x.Length == 2)
            .Select(x => x[1]);
    }

    private Reference? ResolvePackedReference(string name)
    {
        if (!File.Exists(packedReferences))
        {
            return null;
        }

        var reference = File.ReadAllLines(packedReferences)
            .Where(x => !x.StartsWith('#') && !x.StartsWith('^'))
            .Select(x => x.Split(' '))
            .Where(x => x.Length == 2)
            .FirstOrDefault(x => x[1] == name);

        if (reference == null)
        {
            return null;
        }

        return new DirectReference(repository, name, new ObjectId(reference[0]));
    }
}
