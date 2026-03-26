using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.CreatePlaylist;
using TrackYourLife.Modules.Youtube.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;

internal sealed record CreatePlaylistRequest(string Name);

internal sealed class CreatePlaylist(ISender sender) : Endpoint<CreatePlaylistRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreatePlaylistRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new CreatePlaylistCommand(req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.LibraryPlaylists}/{id.Value}");
    }
}
