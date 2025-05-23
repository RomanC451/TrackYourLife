﻿using FluentValidation;
using TrackYourLife.Modules.Users.Infrastructure.Options;

namespace TrackYourLife.Modules.Users.Infrastructure.Validators;

internal sealed class JwtOptionsValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator()
    {
        RuleFor(x => x.Issuer).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Audience).NotEmpty().MaximumLength(100);

        RuleFor(x => x.SecretKey).NotEmpty().MinimumLength(100);

        RuleFor(x => x.MinutesToExpire).NotEmpty().GreaterThan(0);
    }
}
