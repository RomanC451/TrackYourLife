using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.SharedLib.Contracts.Common;

public class PagedList<T>
{
    private PagedList(IEnumerable<T> items, int page, int pageSize, int maxPage)
    {
        Items = items.ToList();
        Page = page;
        PageSize = pageSize;
        MaxPage = maxPage;
    }

    public IReadOnlyCollection<T> Items { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => Page > 1;

    public int MaxPage { get; set; }

    public bool HasNextPage => Page < MaxPage;

    public static Result<PagedList<T>> Create(IEnumerable<T> source, int page, int pageSize)
    {
        var count = source.Count();

        var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var maxPage = (int)Math.Ceiling(count / (double)pageSize);

        maxPage = maxPage == 0 ? 1 : maxPage;

        var result = Result.FirstFailureOrSuccess(
            Ensure.Positive(
                page,
                DomainErrors.ArgumentError.NotPositive(nameof(PagedList<T>), nameof(page))
            ),
            Ensure.Positive(
                pageSize,
                DomainErrors.ArgumentError.NotPositive(nameof(PagedList<T>), nameof(pageSize))
            ),
            Ensure.IsTrue(
                page <= maxPage,
                DomainErrors.ArgumentError.OutOfIndex(nameof(PagedList<T>), nameof(maxPage))
            )
        );

        if (result.IsFailure)
            return Result.Failure<PagedList<T>>(result.Error);

        return Result.Success(new PagedList<T>(items, page, pageSize, maxPage));
    }

    public PagedList<TResult> Map<TResult>(Func<T, TResult> mapFunction)
    {
        var mappedItems = Items.Select(mapFunction).ToList();
        return new PagedList<TResult>(mappedItems, Page, PageSize, MaxPage);
    }
}
