using TrackYourLife.Modules.Reading.Application.Abstraction;
using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetDailyReadingProgress;

internal sealed class GetDailyReadingProgressQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IReadingGoalProvider readingGoalProvider,
    IReadingStatisticsService readingStatisticsService,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetDailyReadingProgressQuery, DailyReadingProgressDto>
{
    public async Task<Result<DailyReadingProgressDto>> Handle(
        GetDailyReadingProgressQuery request,
        CancellationToken cancellationToken
    )
    {
        var date = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var userId = userIdentifierProvider.UserId;

        var sessions = await readingSessionsQuery.GetFinishedByUserIdAsync(
            userId,
            cancellationToken
        );

        var target = await readingGoalProvider.GetTargetPagesForDateAsync(
            userId,
            date,
            cancellationToken
        );

        var progress = readingStatisticsService.CalculateDailyProgress(sessions, date, target);

        return Result.Success(progress);
    }
}
