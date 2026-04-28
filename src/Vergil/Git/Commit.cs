using System.Collections;

namespace Vergil.Git;

public class Commit(IRepository repository, ObjectId id, ObjectId[] parents) : GitObject(id)
{
    private const int TreeEntryLength = 46;

    private const int ParentEntryLength = 48;

    private static readonly byte[] Parent = "parent "u8.ToArray();

    private readonly ParentsCollection parents = new(repository, parents);

    public IEnumerable<Commit> Parents => parents;

    public static Commit Parse(IRepository repository, ObjectId id, ReadOnlySpan<byte> data)
    {
        var parents = new List<ObjectId>();

        var buffer = data.Slice(TreeEntryLength);

        while (buffer.Slice(0, Parent.Length).SequenceEqual(Parent))
        {
            var parent = buffer.Slice(Parent.Length, 40);

            parents.Add(new ObjectId(parent));

            buffer = buffer.Slice(ParentEntryLength);
        }

        return new Commit(repository, id, parents.ToArray());
    }

    public override string ToString()
    {
        return $"{Id.ToString(7)}";
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
