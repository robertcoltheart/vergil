namespace Vergil.Git;

public static class StreamExtensions
{
    public static ObjectId ReadObjectId(this Stream stream)
    {
        var buffer = new byte[20];

        stream.ReadExactly(buffer);

        return new ObjectId(buffer);
    }
}
