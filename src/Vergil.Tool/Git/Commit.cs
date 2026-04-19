using System.Collections;
using System.Text;

namespace Vergil.Tool.Git;

public class Commit(IRepository repository, ObjectId id, ObjectId[] parents, string message) : GitObject(id)
{
    private readonly ParentsCollection parents = new(repository, parents);

    public IEnumerable<Commit> Parents => parents;

    public string Message { get; } = message;

    public Signature Author { get; }

    public Signature Committer { get; }

    public static Commit Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data)
    {
        using var reader = new StringReader(Encoding.UTF8.GetString(data));

        var parents = new List<ObjectId>();

        var line = reader.ReadLine();

        while (line != null)
        {
            if (line.Length == 0)
            {
                break;
            }

            if (line.StartsWith("parent "))
            {
                var parent = new ObjectId(line[7..]);

                parents.Add(parent);
            }

            line = reader.ReadLine();
        }

        var message = reader.ReadToEnd().TrimEnd();

        return new Commit(repository, id, parents.ToArray(), message);
    }

    private class ParentsCollection(IRepository repository, ObjectId[] parents) : IEnumerable<Commit>
    {
        private readonly Lazy<ICollection<Commit>> commitResolver = new(() => LoadParents(repository, parents));

        public IEnumerator<Commit> GetEnumerator()
        {
            return commitResolver.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static ICollection<Commit> LoadParents(IRepository repository, ObjectId[] parents)
        {
            var commits = new List<Commit>();

            foreach (var parent in parents)
            {
                var commit = repository.Lookup<Commit>(parent);

                if (commit == null)
                {
                    throw new InvalidOperationException($"Commit not found: {parent}");
                }

                commits.Add(commit);
            }

            return commits;
        }
    }
}
