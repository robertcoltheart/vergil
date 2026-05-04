using Vergil.Git;

namespace Vergil.Versioning;

public class VersionCalculator(VergilConfiguration configuration)
{
    public VergilVersion Calculate()
    {
        if (string.IsNullOrEmpty(configuration.Path))
        {
            throw new InvalidOperationException("Repository path not specified");
        }

        using var repository = new Repository(configuration.Path);

        var tip = repository.Head.Tip;

        if (tip == null)
        {
            throw new InvalidOperationException("Cannot obtain HEAD");
        }

        var branch = repository.Head.FriendlyName;
        var tagVersion = GetLatestVersion(repository);
        var height = GetCommitHeight(repository, tagVersion!.Tag.Target!.Sha);

        return new VergilVersion
        {
            BranchName = branch,
            Major = tagVersion.Version.Major,
            Minor = tagVersion.Version.Minor,
            Patch = tagVersion.Version.Patch,
            Sha = tip.Id.Sha,
            PreReleaseLabel = "??",
            PreReleaseNumber = 0,
            VersionSourceDistance = height,
            VersionSourceIncrement = string.Empty,
            VersionSourceSemVer = tagVersion.Version.ToString(),
            VersionSourceSha = tagVersion.Tag.Target.Sha
        };
    }

    private TagVersion? GetLatestVersion(IRepository repository)
    {
        return repository.Tags
            .Where(x => SemanticVersion.TryParse(GetTagName(x.FriendlyName), out _))
            .Select(x => new TagVersion(x, SemanticVersion.Parse(GetTagName(x.FriendlyName))))
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();
    }

    private int GetCommitHeight(IRepository repository, string sha)
    {
        var queue = new Queue<(Commit Commit, int Height)>();
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

    private string GetTagName(string value)
    {
        if (value.StartsWith(configuration.TagPrefix))
        {
            return value[configuration.TagPrefix.Length..];
        }

        return string.Empty;
    }
}
