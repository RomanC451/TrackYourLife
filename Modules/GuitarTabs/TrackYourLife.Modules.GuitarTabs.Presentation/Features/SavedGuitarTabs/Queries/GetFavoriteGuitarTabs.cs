using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetFavoriteGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs.Queries;

internal sealed class GetFavoriteGuitarTabs(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get(ApiRoutes.GuitarTabFavorites);
        Description(x =>
            x.Produces<IReadOnlyList<SavedGuitarTabResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetFavoriteGuitarTabsQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(response => response);
    }
}
