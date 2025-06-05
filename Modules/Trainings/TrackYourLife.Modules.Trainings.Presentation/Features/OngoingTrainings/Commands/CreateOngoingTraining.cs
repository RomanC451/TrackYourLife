using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;

internal sealed record CreateOngoingTrainingRequest(TrainingId TrainingId);

internal sealed class CreateOngoingTraining(ISender sender)
    : Endpoint<CreateOngoingTrainingRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<OngoingTrainingsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreateOngoingTrainingRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new CreateOngoingTrainingCommand(req.TrainingId))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync($"/{ApiRoutes.OngoingTrainings}/active-training");
    }
}
