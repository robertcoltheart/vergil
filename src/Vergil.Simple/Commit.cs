namespace Vergil.Simple;

public class Commit(string sha)
{
    public string Sha { get; } = sha;

    public string ShortSha => Sha[..Math.Min(7, Sha.Length)];

    public List<string> Parents { get; } = [];

    public override string ToString()
    {
        return ShortSha;
    }
}
