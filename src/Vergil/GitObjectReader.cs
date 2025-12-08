using System.Buffers;
using System.IO.Compression;

namespace Vergil;

public class GitObjectReader(Stream stream) : IDisposable
{
    private readonly DeflateStream stream = new(stream, CompressionMode.Decompress, false);

    public Commit ReadCommit()
    {
        var buffer = ArrayPool<byte>.Shared.Rent((int)stream.Length);

        try
        {
            stream.Read(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return new Commit(default);
    }

    public void Dispose()
    {
    }
}
