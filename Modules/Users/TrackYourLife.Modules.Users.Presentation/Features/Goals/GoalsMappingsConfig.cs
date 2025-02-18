using Mapster;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Contracts.Goals;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals;

public class GoalsMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Requests to Commands
        config.NewConfig<AddGoalRequest, AddGoalCommand>();
        config.NewConfig<UpdateGoalRequest, UpdateGoalCommand>();
        config.NewConfig<CalculateNutritionGoalsRequest, CalculateNutritionGoalsCommand>();

        //ReadModels to DTOs
        config.NewConfig<GoalReadModel, GoalDto>();
    }
}
