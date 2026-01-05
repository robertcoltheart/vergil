using System.Buffers.Binary;

namespace Vergil.Simple;

public static class BinaryReaderExtensions
{
    private static readonly byte[] numberBuffer = new byte[4];

    extension(BinaryReader reader)
    {
        public int ReadInt32BigEndian()
        {
            reader.ReadExactly(numberBuffer);

            return BinaryPrimitives.ReadInt32BigEndian(numberBuffer);
        }

        public uint ReadUInt32BigEndian()
        {
            reader.ReadExactly(numberBuffer);

            return BinaryPrimitives.ReadUInt32BigEndian(numberBuffer);
        }

        public void ReadExactly(Span<byte> buffer)
        {
            var read = reader.Read(buffer);

            if (read != buffer.Length)
            {
                throw new EndOfStreamException("Unable to read past end of stream");
            }
        }
    }
}
