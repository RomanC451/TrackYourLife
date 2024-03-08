using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Queries.GetUserData;

public sealed class GetUserDataQueryHandler : IQueryHandler<GetUserDataQuery, GetUserDataResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public GetUserDataQueryHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<GetUserDataResult>> Handle(
        GetUserDataQuery _,
        CancellationToken cancellationToken
    )
    {
        var idResult = _authService.GetUserIdFromJwtToken();
        if (idResult.IsFailure)
        {
            return Result.Failure<GetUserDataResult>(idResult.Error);
        }

        UserId userId = new(idResult.Value);

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<GetUserDataResult>(DomainErrors.User.NotFound(userId));
        }

        return new GetUserDataResult(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value
        );
    }
}
