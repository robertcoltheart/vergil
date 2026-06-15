using System.Buffers.Binary;

namespace Vergil.Git;

public ref struct SpanReader(ReadOnlySpan<byte> data)
{
    private readonly ReadOnlySpan<byte> data = data;

    public int Position { get; private set; }

    public int Length { get; } = data.Length;

    public byte ReadByte()
    {
        var value = data[Position];

        Position++;

        return value;
    }

    public uint ReadUInt32()
    {
        var value = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(Position, 4));

        Position += 4;

        return value;
    }

    public int ReadInt32()
    {
        var value = BinaryPrimitives.ReadInt32BigEndian(data.Slice(Position, 4));

        Position += 4;

        return value;
    }

    public ReadOnlySpan<byte> Read(int count)
    {
        var offset = Position;

        Position += count;

        return data.Slice(offset, count);
    }

    public long ReadVariableInt64()
    {
        var value = 0;
        var shift = 0;

        var b = 0x80;

        while ((b & 0x80) != 0)
        {
            b = data[Position++];
            value |= (b & 0x7f) << shift;
            shift += 7;
        }

        return value;
    }
}
