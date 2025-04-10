using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(
    IUserQuery userQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetCurrentUserQuery, UserReadModel>
{
    public async Task<Result<UserReadModel>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await userQuery.GetByIdAsync(userIdentifierProvider.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserReadModel>(
                UserErrors.NotFound(userIdentifierProvider.UserId)
            );
        }

        return Result.Success(user);
    }
}
