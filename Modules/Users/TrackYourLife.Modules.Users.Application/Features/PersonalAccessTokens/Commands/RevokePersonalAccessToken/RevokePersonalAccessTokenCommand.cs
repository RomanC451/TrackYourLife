using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;

public sealed record RevokePersonalAccessTokenCommand(PersonalAccessTokenId Id) : ICommand;
