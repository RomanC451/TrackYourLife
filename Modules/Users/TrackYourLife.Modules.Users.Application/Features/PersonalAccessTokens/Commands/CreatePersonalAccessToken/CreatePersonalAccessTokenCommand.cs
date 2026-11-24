using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;

public sealed record CreatePersonalAccessTokenCommand(string Name)
    : ICommand<CreatedPersonalAccessTokenDto>;
