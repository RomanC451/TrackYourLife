using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Application.Abstractions.Services;

namespace TrackYourLifeDotnet.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<GetUserResponse>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        Guid userId = query.Id;

        if (userId == Guid.Empty)
        {
            var idResult = _authService.GetUserIdFromJwtToken();
            if (idResult.IsFailure)
            {
                return Result.Failure<GetUserResponse>(idResult.Error);
            }

            userId = idResult.Value;
        }

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<GetUserResponse>(DomainErrors.User.NotFound(userId));
        }

        return new GetUserResponse(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value
        );
    }
}
