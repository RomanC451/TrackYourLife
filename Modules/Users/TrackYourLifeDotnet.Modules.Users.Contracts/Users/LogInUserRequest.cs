using System.ComponentModel.DataAnnotations;

namespace TrackYourLife.Modules.Users.Contracts.Users;

public sealed record LogInUserRequest([Required] string Email, [Required] string Password);
