using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public sealed record RegisterUserRequest(
    [Required] string? Email,
    [Required] string? Password,
    [Required] string? FirstName,
    [Required] string? LastName
);
