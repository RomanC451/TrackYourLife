using FluentValidation;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.LogReadingProgress;

public sealed class LogReadingProgressCommandValidator
    : AbstractValidator<LogReadingProgressCommand>
{
    public LogReadingProgressCommandValidator()
    {
        RuleFor(x => x.EndPage).GreaterThan(0);
    }
}
