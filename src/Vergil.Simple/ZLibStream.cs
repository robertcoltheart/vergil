using System.IO.Compression;

namespace Vergil.Simple;

public class ZLibStream : Stream
{
    private readonly DeflateStream stream;

    private long position;

    public ZLibStream(Stream stream)
    {
        this.stream = new DeflateStream(stream, CompressionMode.Decompress, false);

        Initialize();
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => stream.Length;

    public override long Position
    {
        get => position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = stream.Read(buffer, offset, count);
        position += read;

        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    private void Initialize()
    {
        var a = stream.ReadByte();
        var b = stream.ReadByte();
    }
}
