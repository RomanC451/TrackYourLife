using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.CreatePlaylist;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Commands;

internal sealed record CreatePlaylistRequest(string Name);

internal sealed class CreateGuitarPlaylist(ISender sender) : Endpoint<CreatePlaylistRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreatePlaylistRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new CreatePlaylistCommand(req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.GuitarPlaylists}/{id.Value}");
    }
}
