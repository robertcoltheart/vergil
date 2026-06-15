using System.Text.RegularExpressions;
using Vergil.Git;
using Vergil.Pipeline;

namespace Vergil.Versioning.Stages;

public class BranchConfigurationStage(IRepository repository, VergilConfiguration configuration) : IPipelineStage
{
    public void Execute(VersionContext context)
    {
        var branch = FindBranch();

        ExtractVariables(context, branch);

        context.SetBranchConfiguration(branch);
    }

    private VergilBranchConfiguration FindBranch()
    {
        var branch = repository.Head.FriendlyName;

        foreach (var branchConfiguration in configuration.Branhes)
        {
            var pattern = branchConfiguration.Match ?? configuration.Match;

            if (!string.IsNullOrEmpty(pattern) && Regex.IsMatch(branch, pattern))
            {
                return branchConfiguration;
            }
        }

        if (string.IsNullOrEmpty(configuration.Match) || !Regex.IsMatch(branch, configuration.Match))
        {
            throw new InvalidOperationException("No valid branch configuration matches found");
        }

        return configuration;
    }

    private void ExtractVariables(VersionContext context, VergilBranchConfiguration branch)
    {
        var match = Regex.Match(context.Repository.Head.FriendlyName, branch.Match!);

        foreach (var group in match.Groups.Cast<Group>())
        {
            context.Metadata.Variables[group.Name] = group.Value;
        }
    }
}
