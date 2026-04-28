namespace Vergil.Git;

public class Repository : IRepository
{
    public Repository(string path)
    {
        var gitPath = Path.Combine(path, ".git");

        if (!Directory.Exists(gitPath))
        {
            throw new ArgumentException("Path is not a git repository");
        }

        Info = new RepositoryInformation(this, path, gitPath);
        Tags = new TagCollection(this);
        Refs = new ReferenceCollection(this);
        ObjectDatabase = new ObjectDatabase(this, new LooseOdbBackend(gitPath), new PackedOdbBackend(gitPath));
    }

    public RepositoryInformation Info { get; }

    public Branch Head
    {
        get
        {
            var reference = Refs.Head;

            if (reference == null)
            {
                throw new InvalidOperationException("HEAD is missing from repository");
            }

            if (reference is SymbolicReference)
            {
                return new Branch(this, reference);
            }

            return new DetachedHead(this, reference);
        }
    }

    public TagCollection Tags { get; }

    public ReferenceCollection Refs { get; }

    public ObjectDatabase ObjectDatabase { get; }

    public GitObject? Lookup(ObjectId id)
    {
        return ObjectDatabase.Read(id);
    }

    public override string ToString()
    {
        return $"Workdir = \"{Info.WorkingDirectory}\"";
    }
}
