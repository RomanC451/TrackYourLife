using Mapster;
using TrackYourLife.Modules.Users.Application.Users.Commands.LogIn;
using TrackYourLife.Modules.Users.Application.Users.Commands.Register;
using TrackYourLife.Application.Users.Commands.ResendVerificationEmail;
using TrackYourLife.Application.Users.Commands.VerifyEmail;
using TrackYourLife.Application.Users.Queries;
using TrackYourLife.Common.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users;

public class UserMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<LogInUserRequest, LogInUserCommand>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password);

        config
            .NewConfig<RegisterUserRequest, RegisterUserCommand>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName);
    }
}
