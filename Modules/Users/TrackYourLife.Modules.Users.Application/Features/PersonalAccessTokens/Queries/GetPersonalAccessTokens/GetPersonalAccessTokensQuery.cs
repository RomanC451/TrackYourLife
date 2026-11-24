using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Queries.GetPersonalAccessTokens;

public sealed record GetPersonalAccessTokensQuery
    : IQuery<IReadOnlyList<PersonalAccessTokenReadModel>>;
