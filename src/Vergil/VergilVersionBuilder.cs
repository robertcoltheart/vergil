using Vergil.Versioning;

namespace Vergil;

public class VergilVersionBuilder
{
    private string repositoryPath = Environment.CurrentDirectory;

    private VergilConfiguration versionConfiguration = new();

    public VergilVersionBuilder ForPath(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        repositoryPath = path;

        return this;
    }

    public VergilVersionBuilder WithConfiguration(VergilConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        versionConfiguration = configuration;

        return this;
    }

    public VergilVersion Build()
    {
        using var provider = new ServiceProvider(repositoryPath, versionConfiguration);

        var calculator = provider.GetService<VersionCalculator>();

        return calculator.Calculate();
    }
}
