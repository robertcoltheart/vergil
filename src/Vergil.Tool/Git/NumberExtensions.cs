using System.Buffers.Binary;

namespace Vergil.Tool.Git;

public static class NumberExtensions
{
    public static ulong Reverse(this ulong value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static int Reverse(this int value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static uint Reverse(this uint value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    public static long ToInt64(this ulong value)
    {
        return Convert.ToInt64(value);
    }
}
