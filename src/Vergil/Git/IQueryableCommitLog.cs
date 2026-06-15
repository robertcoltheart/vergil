namespace Vergil.Git;

public interface IQueryableCommitLog : ICommitLog
{
    ICommitLog QueryBy(CommitFilter filter);
}
