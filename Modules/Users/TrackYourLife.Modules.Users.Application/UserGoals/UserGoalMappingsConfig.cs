using Mapster;
using TrackYourLife.Modules.Users.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Modules.Users.Application.UserGoals.Commands.UpdateUserGoal;
using TrackYourLife.Application.UserGoals.Queries.GetActiveUserGoalByType;
using TrackYourLife.Common.Contracts.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals;

public class UserGoalMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddUserGoalRequest, AddUserGoalCommand>();

        config.NewConfig<UpdateUserGoalRequest, UpdateUserGoalCommand>();
    }
}
