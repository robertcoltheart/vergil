namespace Vergil.Git;

public class PackFile(string path, PackIndex index) : IDisposable
{
    private readonly MemoryMappedHandle handle = new(path);

    public GitObjectEntry? Read(ObjectId id)
    {
        var offset = index.GetOffset(id);

        if (offset == null)
        {
            return null;
        }

        return Read(offset.Value);
    }

    public void Dispose()
    {
        index.Dispose();
        handle.Dispose();
    }

    private GitObjectEntry? Read(long offset)
    {
        using var stream = handle.OpenStream();
        stream.Seek(offset, SeekOrigin.Begin);

        var header = ReadHeader(stream);

        if (header.Type == PackObjectType.Invalid)
        {
            throw new InvalidOperationException("Invalid pack object type");
        }

        if (header.Type is PackObjectType.OffsetDelta or PackObjectType.ReferenceDelta)
        {
            return ReadDeltifiedObject(header, stream, offset);
        }

        var data = ReadObject(stream, stream.Position, header.Length);

        return new GitObjectEntry(GetObjectType(header.Type), data);
    }

    private GitObjectEntry? ReadDeltifiedObject(ObjectHeader header, Stream stream, long initialOffset)
    {
        var baseObject = ReadBaseObject(header, stream, initialOffset);

        if (baseObject == null)
        {
            throw new InvalidOperationException("Delta object not found");
        }

        var deltaObject = ReadObject(stream, stream.Position, header.Length);

        var delta = new DeltaReader(baseObject.Value.Data.Span, deltaObject);
        var value = delta.ReadArray();

        return new GitObjectEntry(baseObject.Value.Type, value);
    }

    private GitObjectEntry? ReadBaseObject(ObjectHeader header, Stream stream, long initialOffset)
    {
        var ofs = header.Type switch
        {
            PackObjectType.OffsetDelta => initialOffset - ReadOffsetDelta(stream),
            PackObjectType.ReferenceDelta => index.GetOffset(stream.ReadObjectId()),
            _ => throw new InvalidOperationException("Unsupported pack delta type")
        };

        if (ofs == null)
        {
            throw new InvalidOperationException("Delta object not found");
        }

        return Read(ofs.Value);
    }

    private long ReadOffsetDelta(Stream stream)
    {
        var b = stream.ReadByte();

        var value = b & 0x7f;

        while ((b & 0x80) != 0)
        {
            b = stream.ReadByte();
            value = (value + 1) << 7;
            value |= b & 0x7f;
        }

        return value;
    }

    private ObjectHeader ReadHeader(Stream stream)
    {
        var b = stream.ReadByte();

        var type = (PackObjectType)((b >> 4) & 7);
        var length = b & 0x0f;

        var shift = 4;

        while ((b & 0x80) != 0)
        {
            b = stream.ReadByte();
            length |= (b & 0x7f) << shift;
            shift += 7;
        }

        return new ObjectHeader(type, length);
    }

    private byte[] ReadObject(Stream stream, long offset, int count)
    {
        stream.Seek(offset, SeekOrigin.Begin);

        using var deflate = new ZLibStream(stream);

        var buffer = new byte[count];
        deflate.ReadExactly(buffer);

        return buffer;
    }

    private ObjectType GetObjectType(PackObjectType type)
    {
        return type switch
        {
            PackObjectType.Commit => ObjectType.Commit,
            PackObjectType.Tree => ObjectType.Tree,
            PackObjectType.Blob => ObjectType.Blob,
            PackObjectType.Tag => ObjectType.Tag,
            _ => throw new ArgumentException("Invalid pack object type")
        };
    }

    private enum PackObjectType
    {
        Invalid = 0,
        Commit = 1,
        Tree = 2,
        Blob = 3,
        Tag = 4,
        OffsetDelta = 6,
        ReferenceDelta = 7
    }

    private record struct ObjectHeader(PackObjectType Type, int Length);
}
