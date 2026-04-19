namespace Vergil.Tool.Git;

public class RepositoryInformation(IRepository repository, string workingDirectory, string path)
{
    public string Path { get; } = path;

    public string WorkingDirectory { get; } = workingDirectory;

    public bool IsHeadDetached
    {
        get
        {
            var reference = repository.Refs.Head;

            if (reference is SymbolicReference)
            {
                return false;
            }

            return reference?.ResolveToDirectReference() == null;
        }
    }
}
