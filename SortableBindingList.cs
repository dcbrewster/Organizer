using System.ComponentModel;

namespace Organizer;

/// <summary>
/// A sortable binding list that supports sorting by a specified property and direction.
/// </summary>
internal sealed class SortableBindingList<T> : BindingList<T>
{
    private bool _isSorted;
    private ListSortDirection _sortDirection;
    private PropertyDescriptor? _sortProperty;

    public SortableBindingList(IList<T> list)
        : base(list)
    {
    }

    protected override bool SupportsSortingCore => true;

    protected override bool IsSortedCore => _isSorted;

    protected override PropertyDescriptor? SortPropertyCore => _sortProperty;

    protected override ListSortDirection SortDirectionCore => _sortDirection;

    /// <summary>
    /// Applies sorting to the list based on the specified property and direction.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="direction"></param>
    protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
    {
        List<T>? sortedItems = Items.OrderBy(item => property.GetValue(item), new NullableObjectComparer(direction)).ToList();

        RaiseListChangedEvents = false;

        try
        {
            for(int index = 0; index < sortedItems.Count; index++)
            {
                Items[index] = sortedItems[index];
            }
        }
        finally
        {
            RaiseListChangedEvents = true;
        }

        _isSorted = true;
        _sortProperty = property;
        _sortDirection = direction;
        ResetBindings();
    }

    /// <summary>
    /// Removes any sorting applied to the list and resets the sort state.
    /// </summary>
    protected override void RemoveSortCore()
    {
        _isSorted = false;
        _sortProperty = null;
        ResetBindings();
    }

    /// <summary>
    /// A comparer that handles null values and compares objects based on the specified sort direction.
    /// </summary>
    /// <param name="direction"></param>
    private sealed class NullableObjectComparer(ListSortDirection direction) : IComparer<object?>
    {
        public int Compare(object? x, object? y)
        {
            int result = CompareValues(x, y);

            return direction == ListSortDirection.Ascending ? result : -result;
        }

        private static int CompareValues(object? x, object? y)
        {
            if(ReferenceEquals(x, y)) return 0;
            if(x is null) return -1;
            if(y is null) return 1;
            if(x is IComparable comparable) return comparable.CompareTo(y);

            return string.Compare(x.ToString(), y.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}