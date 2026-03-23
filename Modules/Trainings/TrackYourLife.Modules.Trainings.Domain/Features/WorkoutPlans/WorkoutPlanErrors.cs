using System.Net;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public static class WorkoutPlanErrors
{
    public static readonly Func<WorkoutPlanId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(WorkoutPlan));

    public static readonly Func<WorkoutPlanId, Error> NotOwned = id =>
        Error.NotOwned(id, nameof(WorkoutPlan));

    public static readonly Error ActivePlanNotFound = new(
        "WorkoutPlans.ActivePlanNotFound",
        "Active workout plan was not found.",
        (int)HttpStatusCode.NotFound
    );

    public static readonly Error EmptyWorkoutPlan = new(
        "WorkoutPlans.Empty",
        "Workout plan has no workouts."
    );

    public static readonly Error DuplicatedTrainingIds = new(
        "WorkoutPlans.DuplicatedTrainingIds",
        "Workout plan cannot contain duplicated workouts."
    );

    public static readonly Func<TrainingId, Error> TrainingNotFoundById = trainingId =>
        TrainingsErrors.NotFoundById(trainingId);

    public static readonly Func<TrainingId, Error> LastFinishedWorkoutNotInPlan = trainingId =>
        new(
            "WorkoutPlans.LastFinishedWorkoutNotInPlan",
            $"Last finished workout with id {trainingId} does not exist in active workout plan."
        );
}
