using System.IO.Compression;

namespace Vergil.Git;

public class ZLibStream : DeflateStream
{
    public ZLibStream(Stream stream)
        : base(stream, CompressionMode.Decompress, true)
    {
        var compressionMethod = stream.ReadByte();
        var flags = stream.ReadByte();

        if (compressionMethod != 0x78)
        {
            throw new InvalidOperationException("Invalid zlib compression method");
        }

        if (flags is not 0x01 and not 0x9c and not 0x5e and not 0xda)
        {
            throw new InvalidOperationException("Invalid zlib compression level");
        }
    }
}
