namespace Vergil.Git;

public readonly ref struct DeltaReader(ReadOnlySpan<byte> data, ReadOnlySpan<byte> delta)
{
    private readonly ReadOnlySpan<byte> data = data;

    private readonly ReadOnlySpan<byte> delta = delta;

    public byte[] ReadArray()
    {
        var reader = new SpanReader(delta);

        var header = ReadHeader(ref reader);

        var result = new byte[header.TargetLength];
        var offset = 0;

        while (reader.Position < reader.Length)
        {
            var source = GetDeltaInstruction(ref reader);
            var destination = result.AsSpan(offset);

            source.CopyTo(destination);

            offset += source.Length;
        }

        return result;
    }

    private DeltaHeader ReadHeader(ref SpanReader reader)
    {
        var baseLength = reader.ReadVariableInt64();
        var targetLength = reader.ReadVariableInt64();

        return new DeltaHeader(baseLength, targetLength);
    }

    private ReadOnlySpan<byte> GetDeltaInstruction(ref SpanReader reader)
    {
        var cmd = reader.ReadByte();
        var length = 0;

        var instruction = GetInstruction(cmd);

        if (instruction == DeltaInstructionType.Insert)
        {
            length = cmd & 0x7f;

            return reader.Read(length);
        }

        if (instruction == DeltaInstructionType.Copy)
        {
            var offset = 0;

            if ((cmd & 0x01) != 0)
            {
                offset |= reader.ReadByte();
            }

            if ((cmd & 0x02) != 0)
            {
                offset |= reader.ReadByte() << 8;
            }

            if ((cmd & 0x04) != 0)
            {
                offset |= reader.ReadByte() << 16;
            }

            if ((cmd & 0x08) != 0)
            {
                offset |= reader.ReadByte() << 24;
            }

            if ((cmd & 0x10) != 0)
            {
                length |= reader.ReadByte();
            }

            if ((cmd & 0x20) != 0)
            {
                length |= reader.ReadByte() << 8;
            }

            if ((cmd & 0x40) != 0)
            {
                length |= reader.ReadByte() << 16;
            }

            if (length == 0)
            {
                length = 0x10000;
            }

            return data.Slice(offset, length);
        }

        throw new InvalidOperationException("Unsupported instruction type");
    }

    private DeltaInstructionType GetInstruction(byte value)
    {
        return (DeltaInstructionType)((value & 0x80) >> 7);
    }

    private record struct DeltaHeader(long BaseLength, long TargetLength);

    private enum DeltaInstructionType
    {
        Insert = 0,
        Copy = 1
    }
}
