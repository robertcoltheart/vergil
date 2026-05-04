namespace Vergil.Git;

public class ObjectId : IEquatable<ObjectId>
{
    private const int BytesLength = 20;

    private const int HexLength = BytesLength * 2;

    public ObjectId(byte[] rawId)
    {
        if (rawId.Length != BytesLength)
        {
            throw new ArgumentException("Invalid object id");
        }

        RawId = rawId;
        Sha = Convert.ToHexString(rawId).ToLowerInvariant();
    }

    public ObjectId(string sha)
    {
        if (sha.Length != HexLength)
        {
            throw new ArgumentException("Invalid object id");
        }

        RawId = Convert.FromHexString(sha);
        Sha = sha.ToLowerInvariant();
    }

    public ObjectId(ReadOnlySpan<byte> rawId)
    {
        if (rawId.Length != HexLength)
        {
            throw new ArgumentException("Invalid object id");
        }

        RawId = Convert.FromHexString(rawId);
        Sha = Convert.ToHexString(RawId).ToLowerInvariant();
    }

    public string Sha { get; }

    public byte[] RawId { get; }

    public bool Equals(ObjectId? other)
    {
        return other?.Sha == Sha;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ObjectId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Sha);
    }

    public override string ToString()
    {
        return Sha;
    }

    public string ToString(int prefixLength)
    {
        var length = Math.Clamp(prefixLength, 1, HexLength);

        return Sha.Substring(0, Math.Min(Sha.Length, length));
    }
}
