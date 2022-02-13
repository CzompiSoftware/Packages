using System.Collections;

internal class PackageList : ErrorRequest, IList<Package>
{
    private readonly IList<Package> _packages = new List<Package>();

    private PackageList(IList<Package> list) : base()
    {
        this._packages = list;
    }

    public PackageList()
    {
    }

    public Package this[int index] { get => _packages[index]; set => _packages[index] = value; }

    public int Count => _packages.Count;

    public bool IsReadOnly => _packages.IsReadOnly;

    public void Add(Package item)
    {
        _packages.Add(item);
    }

    public void Clear()
    {
        _packages.Clear();
    }

    public bool Contains(Package item)
    {
        return _packages.Contains(item);
    }

    public void CopyTo(Package[] array, int arrayIndex)
    {
        _packages.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Package> GetEnumerator()
    {
        return _packages.GetEnumerator();
    }

    public int IndexOf(Package item)
    {
        return _packages.IndexOf(item);
    }

    public void Insert(int index, Package item)
    {
        _packages.Insert(index, item);
    }

    public bool Remove(Package item)
    {
        return _packages.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _packages.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _packages.GetEnumerator();
    }

    internal static PackageList FromEnumerable(IEnumerable<Package> enumerable) => new PackageList(enumerable.ToList());
    internal static PackageList FromList(IList<Package> list) => new PackageList(list);
}