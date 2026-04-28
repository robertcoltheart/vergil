namespace Vergil.Git;

public readonly struct GitObjectEntry(ObjectType type, Memory<byte> data)
{
    public ObjectType Type { get; } = type;

    public Memory<byte> Data { get; } = data;
}
