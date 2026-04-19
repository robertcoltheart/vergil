using System.IO.MemoryMappedFiles;

namespace Vergil.Tool.Git;

/// <summary>
/// Magic - int
/// Version - int
/// Fanout table - 256 ints
/// Object ids - 
/// </summary>
public class PackFile : IDisposable
{
    private const uint TableOfContents = 0xff744f63;

    private const uint Version = 2;

    private const int HeaderLength = 8;

    private const int FanoutTableSize = 256;

    private const int FanoutTableLength = FanoutTableSize * 4;

    private const int ObjectIdLength = 20;

    private const int ObjectTableOffset = HeaderLength + FanoutTableLength; //oidBase

    private const uint LargeOffsetFlag = 0x80000000;

    private readonly MemoryMappedFile file;

    private readonly MemoryMappedViewAccessor accessor;

    private readonly int[] fanoutTable = new int[FanoutTableSize + 1];

    private readonly byte[] buffer = new byte[ObjectIdLength];

    private int objectCount;

    private bool initialized;

    public PackFile(string fileName)
    {
        file = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        accessor = file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
    }

    public long? GetOffset(ObjectId id)
    {
        EnsureFanoutTable();

        var lo = fanoutTable[id.RawId[0]];
        var hi = fanoutTable[id.RawId[0] + 1];

        if (lo == hi)
        {
            return null;
        }

        var crcTableOffset = ObjectTableOffset + hi * ObjectIdLength; // crcBase
        var offsetTableOffset = crcTableOffset + hi * 4; // ofsBase

        var left = lo;
        var right = hi - 1;

        while (left <= right)
        {
            var mid = (left + right) >>> 1;
            var pos = ObjectTableOffset + mid * ObjectIdLength;

            accessor.ReadArray(pos, buffer, 0, ObjectIdLength);

            var compare = buffer.SequenceCompareTo(id.RawId);

            if (compare == 0)
            {
                return ReadOffset(mid, offsetTableOffset, hi);
            }

            if (compare < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return null;
    }

    public void Dispose()
    {
        accessor.Dispose();
        file.Dispose();
    }

    private long ReadOffset(int index, long offsetBase, int count)
    {
        var value = accessor.ReadUInt32(offsetBase + index * 4).Reverse();

        if ((value & LargeOffsetFlag) == 0)
        {
            return value;
        }

        var largeOffset = value & ~LargeOffsetFlag;

        return accessor.ReadUInt64(offsetBase + count * 4 + largeOffset * 8).Reverse().ToInt64();
    }

    private void EnsureFanoutTable()
    {
        if (!initialized)
        {
            if (accessor.ReadUInt32(0).Reverse() != TableOfContents ||
                accessor.ReadUInt32(4).Reverse() != Version)
            {
                throw new InvalidOperationException("Invalid git index");
            }

            for (var i = 1; i <= FanoutTableSize; i++)
            {
                fanoutTable[i] = accessor.ReadInt32(4 * i + 4).Reverse();
            }

            objectCount = fanoutTable[FanoutTableSize];

            initialized = true;
        }
    }
}
