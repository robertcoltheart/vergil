namespace Vergil.Git;

[Flags]
public enum CommitSortStrategies
{
    // Arbitrary order
    None = 0,

    // Parents before children (can be combined with time)
    Topological = 1,

    // Commit time (can be combined with topological)
    Time = 2,

    // Commits in reverse order (can be combined with any above)
    Reverse = 4
}
