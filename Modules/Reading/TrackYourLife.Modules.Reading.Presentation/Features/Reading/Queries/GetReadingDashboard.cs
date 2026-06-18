using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingDashboard;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading.Queries;

internal sealed class GetReadingDashboard(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/dashboard");
        Group<ReadingGroup>();
        Description(x => x.Produces<ReadingDashboardDto>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetReadingDashboardQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(dashboard => dashboard);
    }
}
