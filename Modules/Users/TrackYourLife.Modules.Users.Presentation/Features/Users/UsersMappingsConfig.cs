using Mapster;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;
using TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users
{
    public class UsersMappingsConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //Requests to Commands
            config.NewConfig<UpdateCurrentUserRequest, UpdateUserCommand>();
        }
    }
}
