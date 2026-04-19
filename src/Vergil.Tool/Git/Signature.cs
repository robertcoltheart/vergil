namespace Vergil.Tool.Git;

public class Signature : IEquatable<Signature>
{
    public string Name { get; }

    public string Email { get; }

    public DateTimeOffset When { get; }

    public bool Equals(Signature? other)
    {
        return other?.Name == Name && other.Email == Email && other.When == When;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Signature);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Email, When);
    }

    public override string ToString()
    {
        return $"{Name} <{Email}>";
    }
}
