using System.ComponentModel.DataAnnotations;

namespace TrackYourLife.Modules.Users.Contracts.Users;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
