using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;

public sealed class CreatePersonalAccessTokenCommandValidator
    : AbstractValidator<CreatePersonalAccessTokenCommand>
{
    public CreatePersonalAccessTokenCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
