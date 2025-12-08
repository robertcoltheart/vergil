using System.IO.Compression;

namespace Vergil.IO;

public class ZLibReader(Stream stream) : IDisposable
{
    private readonly DeflateStream stream = new(stream, CompressionMode.Decompress, false);

    public void Read()
    {

    }

    public void Dispose()
    {
        stream.Dispose();
    }
}
