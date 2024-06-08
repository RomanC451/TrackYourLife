namespace TrackYourLife.Common.Contracts.Common;

public class PagedList<T>
{
    public PagedList() { }

    private PagedList(IEnumerable<T> items, int page, int pageSize, int maxPage)
    {
        Items = items.ToList();
        Page = page;
        PageSize = pageSize;
        MaxPage = maxPage;
    }

    public IReadOnlyCollection<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => Page > 1;

    public int MaxPage { get; set; }

    public bool HasNextPage => Page < MaxPage;

    public static PagedList<T> Create(IEnumerable<T> source, int page, int pageSize)
    {
        if (page <= 0)
            page = 1;

        var count = source.Count();

        var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var maxPage = (int)Math.Ceiling(count / (double)pageSize);

        return new PagedList<T>(items, page, pageSize, maxPage);
    }
}
