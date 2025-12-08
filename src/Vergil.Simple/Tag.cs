namespace Vergil.Simple;

public class Tag(string name, string sha)
{
    public string Name { get; } = name;

    public string Sha { get; } = sha;
}
