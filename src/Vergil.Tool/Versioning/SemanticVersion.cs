using System.Text;

namespace Vergil.Tool.Versioning;

public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>
{
    public int Major { get; private set; }

    public int Minor { get; private set; }

    public int Patch { get; private set; }

    public IReadOnlyList<string> PrereleaseLabels { get; private set; } = [];

    public bool IsPrerelease => PrereleaseLabels.Any();

    public string Metadata { get; private set; } = string.Empty;

    public static SemanticVersion Parse(string value)
    {
        if (!TryParse(value, out var version))
        {
            throw new ArgumentException("Invalid version", nameof(value));
        }

        return version;
    }

    public static bool TryParse(string value, out SemanticVersion version)
    {
        version = new SemanticVersion();

        var metadataIndex = value.IndexOf('+');

        if (metadataIndex >= 0)
        {
            version.Metadata = value[(metadataIndex + 1)..];

            if (!version.Metadata.Split('.').All(x => IsValidIdentifier(x, true)))
            {
                return false;
            }

            value = value[..metadataIndex];
        }

        var prereleaseIndex = value.IndexOf('-');

        if (prereleaseIndex >= 0)
        {
            version.PrereleaseLabels = value[(prereleaseIndex + 1)..].Split('.');

            if (!version.PrereleaseLabels.All(x => IsValidIdentifier(x, false)))
            {
                return false;
            }

            value = value[..prereleaseIndex];
        }

        var versionParts = value.Split('.');

        if (versionParts.Length != 3)
        {
            return false;
        }

        if (!TryParseVersionPart(versionParts[2], out var patch) || patch < 0)
        {
            return false;
        }

        version.Patch = patch;

        if (!TryParseVersionPart(versionParts[1], out var minor) || minor < 0)
        {
            return false;
        }

        version.Minor = minor;

        if (!TryParseVersionPart(versionParts[0], out var major) || major < 0)
        {
            return false;
        }

        version.Major = major;

        return true;
    }

    private static bool TryParseVersionPart(string value, out int result)
    {
        result = 0;

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (value.Length > 1 && value.StartsWith('0'))
        {
            return false;
        }

        return int.TryParse(value, out result);
    }

    private static bool IsValidIdentifier(string value, bool allowLeadingZero)
    {
        if (!allowLeadingZero && value.Length > 1 && value.StartsWith('0'))
        {
            return false;
        }

        return !string.IsNullOrEmpty(value) &&
               value.All(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-');
    }

    public override string ToString()
    {
        var value = new StringBuilder();
        value.Append($"{Major}.{Minor}.{Patch}");

        if (IsPrerelease)
        {
            value.Append('-').Append(string.Join('.', PrereleaseLabels));
        }

        if (!string.IsNullOrEmpty(Metadata))
        {
            value.Append('+').Append(Metadata);
        }

        return value.ToString();
    }

    public bool Equals(SemanticVersion other)
    {
        return CompareTo(other) == 0;
    }

    public int CompareTo(SemanticVersion other)
    {
        return Compare(this, other);
    }

    private static int Compare(SemanticVersion x, SemanticVersion y)
    {
        var major = x.Major.CompareTo(y.Major);

        if (major != 0)
        {
            return major;
        }

        var minor = x.Minor.CompareTo(y.Minor);

        if (minor != 0)
        {
            return minor;
        }

        var patch = x.Patch.CompareTo(y.Patch);

        if (patch != 0)
        {
            return patch;
        }

        var prerelease = ComparePrereleaseLabels(x, y);

        if (prerelease != 0)
        {
            return prerelease;
        }

        return 0;
    }

    private static int ComparePrereleaseLabels(SemanticVersion x, SemanticVersion y)
    {
        if (!x.IsPrerelease && y.IsPrerelease)
        {
            return 1;
        }

        if (x.IsPrerelease && !y.IsPrerelease)
        {
            return -1;
        }

        for (var i = 0; i < Math.Min(x.PrereleaseLabels.Count, y.PrereleaseLabels.Count); i++)
        {
            var compare = ComparePrerelease(x.PrereleaseLabels[i], y.PrereleaseLabels[i]);

            if (compare != 0)
            {
                return compare;
            }
        }

        return x.PrereleaseLabels.Count.CompareTo(y.PrereleaseLabels.Count);
    }

    private static int ComparePrerelease(string? x, string? y)
    {
        var xIsNumeric = int.TryParse(x, out var xNumeric);
        var yIsNumeric = int.TryParse(y, out var yNumeric);

        if (xIsNumeric && yIsNumeric)
        {
            return xNumeric.CompareTo(yNumeric);
        }

        if (xIsNumeric || yIsNumeric)
        {
            return xIsNumeric
                ? -1
                : 1;
        }

        return StringComparer.OrdinalIgnoreCase.Compare(x, y);
    }
}
