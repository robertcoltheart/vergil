using System.Collections;

namespace Vergil.Tool.Git;

public class CommitLog : ICommitLog
{
    public IEnumerator<Commit> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
