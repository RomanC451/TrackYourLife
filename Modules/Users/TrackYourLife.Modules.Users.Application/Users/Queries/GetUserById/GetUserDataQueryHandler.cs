using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Contracts.Users;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

namespace TrackYourLife.Modules.Users.Application.Users.Queries.GetUserById;

public sealed class GetUserDataQueryHandler : IQueryHandler<GetUserDataQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;

    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetUserDataQueryHandler(
        IUserRepository userRepository,
        IAuthService authService,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _userRepository = userRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result<UserResponse>> Handle(
        GetUserDataQuery _,
        CancellationToken cancellationToken
    )
    {
        Domain.Users.User? user = await _userRepository.GetByIdAsync(
            _userIdentifierProvider.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure<UserResponse>(
                DomainErrors.User.NotFound(_userIdentifierProvider.UserId)
            );
        }

        return new UserResponse(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value
        );
    }
}
