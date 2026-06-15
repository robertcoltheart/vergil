using System.Collections;

namespace Vergil.Git;

public class CommitLog(IRepository repository, CommitFilter? filter = null) : IQueryableCommitLog
{
    private readonly CommitFilter commitFilter = filter ?? new CommitFilter();

    public CommitSortStrategies SortedBy => commitFilter.SortBy;

    public ICommitLog QueryBy(CommitFilter filter)
    {
        return new CommitLog(repository, filter);
    }

    public IEnumerator<Commit> GetEnumerator()
    {
        return new CommitEnumerator(repository, commitFilter);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class CommitEnumerator(IRepository repository, CommitFilter filter) : IEnumerator<Commit>
    {
        private ObjectId? current;

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            current = null;
        }

        public Commit Current
        {
            get
            {
                ArgumentNullException.ThrowIfNull(current);

                return repository.Lookup<Commit>(current) ?? throw new InvalidOperationException("Commit not found");
            }
        }

        object? IEnumerator.Current => current;

        public void Dispose()
        {
        }
    }
}
