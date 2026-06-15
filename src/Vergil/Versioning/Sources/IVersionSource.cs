namespace Vergil.Versioning.Sources;

public interface IVersionSource
{
    IEnumerable<TagVersion> GetVersions();
}
