namespace Vergil.Simple;

public class PackIndex
{
    private const int Fanout = 256;

    private const int ObjectIdLength = 20;

    private readonly uint[] fanout = new uint[Fanout];

    private readonly int[][] names = new int[Fanout][];

    private readonly byte[][] checksums = new byte[Fanout][];

    private readonly byte[] packChecksum = new byte[20];

    private readonly byte[] indexChecksum = new byte[20];

    private readonly byte[][] offsets32 = new byte[Fanout][];

    private readonly byte[] offsets64;

    public PackIndex(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < Fanout; i++)
        {
            fanout[i] = reader.ReadUInt32BigEndian();
        }

        for (var i = 0; i < Fanout; i++)
        {
            var bucketCount = i == 0
                ? fanout[i]
                : fanout[i] - fanout[i - 1];

            var nameLength = bucketCount * ObjectIdLength;

            names[i] = new int[nameLength >> 2];
            offsets32[i] = new byte[bucketCount * 4];
            checksums[i] = new byte[bucketCount * 4];

            for (var j = 0; j < names[i].Length; j++)
            {
                names[i][j] = reader.ReadInt32BigEndian();
            }
        }

        for (var i = 0; i < Fanout; i++)
        {
            reader.ReadExactly(checksums[i]);
        }

        for (var i = 0; i < Fanout; i++)
        {
            reader.ReadExactly(offsets32[i]);
        }

        offsets64 = new byte[GetInt64OffsetCount()];

        reader.ReadExactly(offsets64);
        reader.ReadExactly(packChecksum);
        reader.ReadExactly(indexChecksum);
    }

    public long FindOffset()

    private int GetInt64OffsetCount()
    {
        var count = 0;

        for (var i = 0; i < Fanout; i++)
        {
            for (var j = 0; j < offsets32[i].Length; j += 4)
            {
                if ((offsets32[i][j] & 0x80) != 0)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
