using System.IO.MemoryMappedFiles;

namespace Vergil.Git;

public unsafe class MemoryMappedHandle : IDisposable
{
    private readonly MemoryMappedFile file;

    private readonly MemoryMappedViewAccessor view;

    private readonly byte* pointer;

    public MemoryMappedHandle(string path)
    {
        file = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        view = file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        view.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
    }

    public Stream OpenStream()
    {
        return new MemoryMappedStream(view);
    }

    public ReadOnlySpan<byte> GetSpan(long offset, int length)
    {
        return new ReadOnlySpan<byte>(pointer + offset, length);
    }

    public void Dispose()
    {
        view.SafeMemoryMappedViewHandle.ReleasePointer();

        view.Dispose();
        file.Dispose();
    }
}
