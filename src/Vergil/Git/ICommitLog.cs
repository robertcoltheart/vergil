namespace Vergil.Git;

public interface ICommitLog : IEnumerable<Commit>
{
    CommitSortStrategies SortedBy { get; }
}
