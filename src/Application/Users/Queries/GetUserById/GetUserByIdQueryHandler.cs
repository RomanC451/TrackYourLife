using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserResponse>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        User? user = await _userRepository.GetByIdAsync(query.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<GetUserResponse>(DomainErrors.User.NotFound(query.Id));
        }

        return new GetUserResponse(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value
        );
    }
}
