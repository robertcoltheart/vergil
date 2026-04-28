using System.IO.MemoryMappedFiles;

namespace Vergil.Git;

public unsafe class MemoryMappedStream : Stream
{
    private readonly MemoryMappedViewAccessor accessor;

    private readonly byte* pointer;

    private bool disposed;

    public MemoryMappedStream(MemoryMappedViewAccessor accessor)
    {
        this.accessor = accessor;

        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => accessor.Capacity;

    public override long Position { get; set; }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateStream();

        var read = Convert.ToInt32(Math.Min(count, Length - Position));

        var span = new Span<byte>(pointer + Position, read);
        span.CopyTo(buffer);

        Position += read;

        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ValidateStream();

        var position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => Length - offset,
            _ => throw new ArgumentException("Invalid seek origin")
        };

        if (position > Length)
        {
            position = Length;
        }

        if (position < 0)
        {
            throw new IOException("Cannot seek before start of stream");
        }

        Position = position;

        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            disposed = true;
        }
    }

    private void ValidateStream()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(MemoryMappedStream));
        }
    }
}
