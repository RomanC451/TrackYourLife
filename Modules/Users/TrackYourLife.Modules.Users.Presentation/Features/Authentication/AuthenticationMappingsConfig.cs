using Mapster;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication;

internal sealed class AuthenticationMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Requests to Commands
        config.NewConfig<RegisterUserRequest, RegisterUserCommand>();
        config.NewConfig<LogInUserRequest, LogInUserCommand>();
    }
}
