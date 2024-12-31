using Mapster;
using MapsterMapper;
using TrackYourLife.Modules.Users.Application.Core.Abstraction;

namespace TrackYourLife.Modules.Users.Application.Core;

public class UsersMapper(IServiceProvider serviceProvider, TypeAdapterConfig config)
    : ServiceMapper(serviceProvider, config),
        IUsersMapper;
