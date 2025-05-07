using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Users;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users;

internal static class UsersMappingsExtensions
{
    public static UserDto ToDto(this UserReadModel user)
    {
        return new UserDto(user.Id, user.Email, user.FirstName, user.LastName);
    }
}
