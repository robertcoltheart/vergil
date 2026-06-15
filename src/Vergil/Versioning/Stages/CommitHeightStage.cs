using Vergil.Git;
using Vergil.Pipeline;

namespace Vergil.Versioning.Stages;

public class CommitHeightStage(IRepository repository) : IPipelineStage
{
    public void Execute(VersionContext context)
    {
        var height = CalculateHeight(context.BaseVersion.Sha);

        context.SetCommitHeight(height);
    }

    private int CalculateHeight(string sha)
    {
        var queue = new Queue<(Commit commit, int Height)>();
        var visited = new HashSet<string>();

        queue.Enqueue((repository.Head.Tip!, 0));
        visited.Add(repository.Head.Tip!.Sha);

        while (queue.Count > 0)
        {
            var (current, height) = queue.Dequeue();

            if (current.Sha == sha)
            {
                return height;
            }

            foreach (var parent in current.Parents.Reverse())
            {
                if (visited.Add(parent.Sha))
                {
                    queue.Enqueue((parent, height + 1));
                }
            }
        }

        return 0;
    }
}
