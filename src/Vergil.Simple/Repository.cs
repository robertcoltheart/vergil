namespace Vergil.Simple;

public class Repository(string directory)
{
    private const string SymbolicRef = "ref: ";

    public Commit GetHead()
    {
        var sha = Resolve("HEAD");

        if (string.IsNullOrEmpty(sha))
        {
            throw new InvalidOperationException("HEAD missing from repository");
        }

        return ReadCommit(sha);
    }

    public IEnumerable<Tag> GetTags()
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

    private Commit ReadCommit(string sha)
    {
        var path = Path.Combine(directory, "objects", sha[..2], sha[2..]);

        if (!File.Exists(path))
        {
            throw new InvalidOperationException("Object database corrupted");
        }

        using var reader = new ObjectReader(path);

        var commit = new Commit(sha);

        var (type, reference) = reader.Read();

        while (!string.IsNullOrEmpty(type))
        {
            if (type == "parent")
            {
                commit.Parents.Add(reference);
            }

            (type, reference) = reader.Read();
        }

        return commit;
    }

    private string? Resolve(string name)
    {
        return ResolveLoose(name) ?? ResolvePacked(name);
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
        var path = Path.Combine(directory, "packed-refs");

        if (File.Exists(path))
        {
            using var reader = new StreamReader(File.OpenRead(path));

            var line = reader.ReadLine();

            while (line != null)
            {
                if (!string.IsNullOrEmpty(line) && line[0] != (byte)'#')
                {
                    var id = line[..40];
                    var refName = line[(id.Length + 1)..];

                    if (refName == name)
                    {
                        return "??";
                    }
                }

                line = reader.ReadLine();
            }
        }

        return null;
    }
}
