namespace Vergil.Git;

public interface IRepository
{
    RepositoryInformation Info { get; }

    Branch Head { get; }

    TagCollection Tags { get; }

    ReferenceCollection Refs { get; }

    ObjectDatabase ObjectDatabase { get; }

    GitObject? Lookup(ObjectId id);
}
