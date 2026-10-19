using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylistById;

internal sealed class GetPlaylistByIdQueryValidator : AbstractValidator<GetPlaylistByIdQuery>
{
    public GetPlaylistByIdQueryValidator()
    {
        RuleFor(x => x.GuitarPlaylistId).NotEqual(GuitarPlaylistId.Empty);
    }
}
