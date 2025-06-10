using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;

internal sealed record CreateTrainingRequest(
    string Name,
    List<ExerciseId> ExercisesIds,
    string? Description,
    int Duration = 0,
    int RestSeconds = 0
);

internal sealed class CreateTraining(ISender sender) : Endpoint<CreateTrainingRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreateTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new CreateTrainingCommand(
                    Name: req.Name,
                    ExercisesIds: req.ExercisesIds,
                    Duration: req.Duration,
                    RestSeconds: req.RestSeconds,
                    Description: req.Description
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(trainingId => $"/{ApiRoutes.Trainings}/{trainingId.Value}");
    }
}
