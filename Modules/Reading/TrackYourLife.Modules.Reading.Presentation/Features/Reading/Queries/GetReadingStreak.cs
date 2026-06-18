using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingStreak;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading.Queries;

internal sealed class GetReadingStreak(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/streak");
        Group<ReadingGroup>();
        Description(x => x.Produces<ReadingStreakDto>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetReadingStreakQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(streak => streak);
    }
}
