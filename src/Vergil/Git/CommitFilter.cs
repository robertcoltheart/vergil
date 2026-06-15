using System.Collections;

namespace Vergil.Git;

public class CommitFilter
{
    private static readonly Type[] ValidTypes =
    [
        typeof(string),
        typeof(ObjectId),
        typeof(Commit),
        typeof(TagAnnotation),
        typeof(Tag),
        typeof(Branch),
        typeof(DetachedHead),
        typeof(Reference),
        typeof(DirectReference),
        typeof(SymbolicReference)
    ];

    public CommitSortStrategies SortBy { get; set; } = CommitSortStrategies.Time;

    public object IncludeReachableFrom { get; set; } = "HEAD";

    public IReadOnlyCollection<object> Since => GetList(IncludeReachableFrom).ToArray();

    public object? ExcludeReachableFrom { get; set; }

    public IReadOnlyCollection<object> Until => GetList(ExcludeReachableFrom).ToArray();

    public bool FirstParentOnly { get; set; }

    private IEnumerable<object> GetList(object? value)
    {
        var items = new List<object>();

        if (value == null)
        {
            return items;
        }

        if (ValidTypes.Contains(value.GetType()))
        {
            items.Add(value);
        }
        else if (value is IEnumerable enumerable)
        {
            items.AddRange(enumerable.Cast<object>());
        }

        return items;
    }
}
