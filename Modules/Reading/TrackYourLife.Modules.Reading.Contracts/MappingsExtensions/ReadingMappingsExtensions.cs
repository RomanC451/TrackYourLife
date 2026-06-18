using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Contracts.MappingsExtensions;

public static class ReadingMappingsExtensions
{
    public static bool SuggestMarkAsFinished(this BookReadModel book) =>
        book.CurrentPage >= book.TotalPages;

    public static bool SuggestMarkAsFinished(this Book book) =>
        book.SuggestMarkAsFinished();
}
