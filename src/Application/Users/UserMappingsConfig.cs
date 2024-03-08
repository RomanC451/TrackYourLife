using Mapster;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;
using TrackYourLifeDotnet.Application.Users.Commands.Register;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;
using TrackYourLifeDotnet.Application.Users.Commands.Update;
using TrackYourLifeDotnet.Application.Users.Commands.VerifyEmail;
using TrackYourLifeDotnet.Application.Users.Queries;

namespace TrackYourLifeDotnet.Application.Users;

public class UserMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<LoginUserRequest, LoginUserCommand>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password);

        config.NewConfig<LoginUserResult, LoginUserResponse>();

        config
            .NewConfig<RegisterUserRequest, RegisterUserCommand>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName);

        config.NewConfig<RegisterUserResult, RegisterUserResponse>();

        config.NewConfig<UpdateUserResult, UpdateUserResponse>();

        config
            .NewConfig<RefreshJwtTokenResponse, RefreshJwtTokenResponse>()
            .Map(dest => dest.NewJwtTokenString, src => src.NewJwtTokenString);

        config
            .NewConfig<ResendEmailVerificationRequest, ResendEmailVerificationCommand>()
            .Map(dest => dest.Email, src => src.Email);

        config
            .NewConfig<VerifyEmailRequest, VerifyEmailCommand>()
            .Map(dest => dest.VerificationToken, src => src.token);

        config.NewConfig<VerifyEmailResult, VerifyEmailResponse>();

        config.NewConfig<GetUserDataResult, GetUserDataResponse>();
    }
}
