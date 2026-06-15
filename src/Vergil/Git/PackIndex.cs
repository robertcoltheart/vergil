using System.Buffers.Binary;

namespace Vergil.Git;

public class PackIndex : IDisposable
{
    private const int ObjectIdLength = 20;

    private const uint LargeOffsetFlag = 0x80000000;

    private readonly MemoryMappedHandle handle;

    private readonly PackIndexHeader header;

    private readonly Dictionary<ObjectId, long?> offsets = [];

    public PackIndex(string path)
    {
        handle = new MemoryMappedHandle(path);
        header = new PackIndexHeader(new SpanReader(handle.GetSpan(0, 8 + 256 * 4)));
    }

    public long? GetOffset(ObjectId id)
    {
        if (offsets.TryGetValue(id, out var offset))
        {
            return offset;
        }

        var firstByte = id.RawId[0];

        var start = header.FanoutTable[firstByte];
        var end = header.FanoutTable[firstByte + 1];
        var table = handle.GetSpan(header.ObjectTableOffset + start * 20, (end - start + 1) * 20);

        var lo = 0;
        var hi = end - start;

        while (lo <= hi)
        {
            var mid = (lo + hi) / 2;

            var buffer = table.Slice(mid * ObjectIdLength, 20);

            var compare = buffer.SequenceCompareTo(id.RawId);

            if (compare == 0)
            {
                return offsets[id] = ReadOffset(start + mid);
            }

            if (compare < 0)
            {
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        offsets[id] = null;

        return null;
    }

    public void Dispose()
    {
        handle.Dispose();
    }

    private long ReadOffset(int index)
    {
        var value = BinaryPrimitives.ReadUInt32BigEndian(handle.GetSpan(header.OffsetTableOffset + index * 4, 4));

        if ((value & LargeOffsetFlag) == 0)
        {
            return value;
        }

        var largeOffset = value & ~LargeOffsetFlag;

        return BinaryPrimitives.ReadInt64BigEndian(handle.GetSpan(header.LargeOffsetTableOffset + largeOffset * 8, 8));
    }
}
