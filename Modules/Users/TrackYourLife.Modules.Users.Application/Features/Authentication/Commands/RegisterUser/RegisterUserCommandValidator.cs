using FluentValidation;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private RegisterUserCommandValidator() { }

    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage(command => Email.Create(command.Email).Error.Message)
            .DependentRules(() =>
            {
                RuleFor(x => x.Email)
                    .MustAsync(
                        async (email, cancellationToken) =>
                        {
                            return await userRepository.IsEmailUniqueAsync(
                                Email.Create(email).Value,
                                cancellationToken
                            );
                        }
                    )
                    .WithMessage(UserErrors.Email.AlreadyUsed.Message);
            });

        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(password => Password.Create(password).IsSuccess)
            .WithMessage(command => Password.Create(command.Password).Error.Message);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Must(firstName => Name.Create(firstName).IsSuccess)
            .WithMessage(command => Name.Create(command.FirstName).Error.Message);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Must(lastName => Name.Create(lastName).IsSuccess)
            .WithMessage(command => Name.Create(command.LastName).Error.Message);
    }
}
