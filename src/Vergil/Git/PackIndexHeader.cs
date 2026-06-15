namespace Vergil.Git;

public class PackIndexHeader(SpanReader reader)
{
    private const uint TableOfContents = 0xff744f63;

    private const uint Version = 2;

    private const int FanoutTableLength = 256;

    private const int ObjectIdLength = 20;

    public int[] FanoutTable { get; } = ReadFanoutTable(reader);

    public long ObjectTableOffset => 8 + FanoutTableLength * 4;

    public long CrcTableOffset => ObjectTableOffset + ObjectCount * ObjectIdLength;

    public long OffsetTableOffset => CrcTableOffset + ObjectCount * 4;

    public long LargeOffsetTableOffset => OffsetTableOffset + ObjectCount * 4;

    public int ObjectCount => FanoutTable[FanoutTableLength];

    private static int[] ReadFanoutTable(SpanReader reader)
    {
        if (reader.ReadUInt32() != TableOfContents ||
            reader.ReadUInt32() != Version)
        {
            throw new InvalidOperationException("Invalid pack index");
        }

        var table = new int[FanoutTableLength + 1];

        for (var i = 1; i <= FanoutTableLength; i++)
        {
            table[i] = reader.ReadInt32();
        }

        return table;
    }
}
