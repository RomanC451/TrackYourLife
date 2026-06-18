using MassTransit;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.Abstraction;

public interface IReadingGoalProvider
{
    Task<int?> GetTargetPagesForDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    );
}

internal sealed class ReadingGoalProvider(IBus bus) : IReadingGoalProvider
{
    public async Task<int?> GetTargetPagesForDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    )
    {
        var client = bus.CreateRequestClient<GetReadingGoalByUserIdRequest>();
        var response = await client.GetResponse<GetReadingGoalByUserIdResponse>(
            new GetReadingGoalByUserIdRequest(userId, date),
            cancellationToken
        );

        if (response.Message.Errors.Count > 0)
        {
            return null;
        }

        return response.Message.TargetPages;
    }
}
