namespace Vergil.Git;

public interface IRepository : IDisposable
{
    RepositoryInformation Info { get; }

    Branch Head { get; }

    TagCollection Tags { get; }

    ReferenceCollection Refs { get; }

    ObjectDatabase ObjectDatabase { get; }

    IQueryableCommitLog Commits { get; }

    GitObject? Lookup(ObjectId id);
}
