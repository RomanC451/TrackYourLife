using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetPlaylistSongs;

internal sealed class GetPlaylistSongsQueryValidator : AbstractValidator<GetPlaylistSongsQuery>
{
    public GetPlaylistSongsQueryValidator()
    {
        RuleFor(x => x.GuitarPlaylistId).NotEqual(GuitarPlaylistId.Empty);
    }
}
