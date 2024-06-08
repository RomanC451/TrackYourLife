using System.ComponentModel.DataAnnotations;

namespace TrackYourLife.Modules.Users.Contracts.Users;

public sealed record UpdateUserRequest(string FirstName, string LastName);
