using System.IO.Compression;

namespace Vergil.Simple;

public class ObjectReader(string path) : IDisposable
{
    private readonly ObjectBuffer buffer = ReadBuffer(path);

    private int index;

    public string Type => buffer.Type;

    public (string Type, string Reference) Read()
    {
        if (index >= buffer.Lines.Length)
        {
            return (string.Empty, string.Empty);
        }

        var parts = buffer.Lines[index++].Split(' ');

        return (parts[0], parts[1]);
    }

    public void Dispose()
    {
    }

    private static ObjectBuffer ReadBuffer(string path)
    {
        using var stream = File.OpenRead(path);
        using var deflate = new DeflateStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(deflate);

        stream.Seek(2, SeekOrigin.Begin);

        var contents = reader.ReadToEnd();

        var end = contents.IndexOf('\0');
        var header = contents[..end].Split(' ');
        var data = contents[(end + 1)..];

        return new ObjectBuffer(ParseData(data).ToArray(), header[0]);
    }

    private static IEnumerable<string> ParseData(string contents)
    {
        using var reader = new StringReader(contents);

        var line = reader.ReadLine();

        while (!string.IsNullOrEmpty(line))
        {
            yield return line;

            line = reader.ReadLine();
        }
    }

    private class ObjectBuffer(string[] lines, string type)
    {
        public string[] Lines { get; } = lines;

        public string Type { get; } = type;
    }
}
