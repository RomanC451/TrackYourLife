using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.LogReadingProgress;

public sealed record LogReadingProgressCommand(int EndPage, DateOnly? SessionDate)
    : ICommand<LogReadingProgressDto>;
