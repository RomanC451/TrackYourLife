using System.ComponentModel;
using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingPagesHistory;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading.Queries;

internal sealed record GetReadingPagesHistoryRequest
{
    [QueryParam]
    public DateOnly? StartDate { get; init; }

    [QueryParam]
    public DateOnly? EndDate { get; init; }

    [QueryParam]
    [DefaultValue(ReadingOverviewType.Weekly)]
    public ReadingOverviewType OverviewType { get; init; } = ReadingOverviewType.Weekly;
}

internal sealed class GetReadingPagesHistory(ISender sender)
    : Endpoint<GetReadingPagesHistoryRequest, IResult>
{
    public override void Configure()
    {
        Get("/pages-history");
        Group<ReadingGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<ReadingPagesDataPointDto>>(StatusCodes.Status200OK)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetReadingPagesHistoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetReadingPagesHistoryQuery(req.StartDate, req.EndDate, req.OverviewType)
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(data => data.ToList());
    }
}
