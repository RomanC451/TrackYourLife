using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Queries.GetPersonalAccessTokens;

public sealed class GetPersonalAccessTokensQueryValidator
    : AbstractValidator<GetPersonalAccessTokensQuery>
{
    public GetPersonalAccessTokensQueryValidator()
    {
        // No validation rules needed as the query has no parameters
    }
}
