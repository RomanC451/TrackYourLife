using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetDailyReadingProgress;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading.Queries;

internal sealed record GetDailyReadingProgressRequest
{
    [QueryParam]
    public DateOnly? Date { get; init; }
}

internal sealed class GetDailyReadingProgress(ISender sender)
    : Endpoint<GetDailyReadingProgressRequest, IResult>
{
    public override void Configure()
    {
        Get("/daily-progress");
        Group<ReadingGroup>();
        Description(x => x.Produces<DailyReadingProgressDto>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(
        GetDailyReadingProgressRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetDailyReadingProgressQuery(req.Date))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(progress => progress);
    }
}
