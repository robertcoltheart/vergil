namespace Vergil.Tool.Git;

public static class RepositoryExtensions
{
    public static T? Lookup<T>(this IRepository repository, ObjectId id)
        where T : GitObject
    {
        return repository.Lookup(id) as T;
    }

    public static T? Lookup<T>(this IRepository repository, string objectish)
        where T : GitObject
    {
        return repository.Lookup(objectish) as T;
    }
}
