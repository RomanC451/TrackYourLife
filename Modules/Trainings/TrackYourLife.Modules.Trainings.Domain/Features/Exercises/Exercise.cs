using System.Text.Json;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public sealed class Exercise : Entity<ExerciseId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public List<string> MuscleGroups { get; private set; } = new();
    public Difficulty Difficulty { get; private set; } = Difficulty.Easy;
    public string? PictureUrl { get; private set; } = string.Empty;
    public string? VideoUrl { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public string? Equipment { get; private set; } = string.Empty;
    public string ExerciseSetsJson { get; private set; } = "[]";

    private List<ExerciseSet> GetExerciseSets()
    {
        if (string.IsNullOrEmpty(ExerciseSetsJson) || ExerciseSetsJson == "[]")
        {
            return new List<ExerciseSet>();
        }

        var sets = JsonSerializer.Deserialize<List<ExerciseSet>>(ExerciseSetsJson);
        return sets?.OrderBy(x => x.OrderIndex).ToList() ?? new();
    }

    public IReadOnlyList<ExerciseSet> ExerciseSets
    {
        get => GetExerciseSets();
        private set =>
            ExerciseSetsJson =
                value == null || !value.Any() ? "[]" : JsonSerializer.Serialize(value.ToList());
    }

    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; }

    private Exercise()
        : base() { }

#pragma warning disable S107

    private Exercise(
        ExerciseId id,
        UserId userId,
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        string? pictureUrl,
        string? videoUrl,
        string? description,
        string? equipment,
        List<ExerciseSet> sets,
        DateTime createdOn
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        MuscleGroups = muscleGroups;
        Difficulty = difficulty;
        PictureUrl = pictureUrl;
        VideoUrl = videoUrl;
        Description = description;
        Equipment = equipment;
        ExerciseSets = sets;
        CreatedOnUtc = createdOn;
    }

    public static Result<Exercise> Create(
        ExerciseId id,
        UserId userId,
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        string? pictureUrl,
        string? videoUrl,
        string? description,
        string? equipment,
        List<ExerciseSet> sets,
        DateTime createdOn
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(id))),
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(name))),
            Ensure.NotEmpty(
                createdOn,
                DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(createdOn))
            ),
            Ensure.NotEmptyList(
                sets,
                DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(sets))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Exercise>(result.Error);
        }

        return Result.Success(
            new Exercise(
                id,
                userId,
                name,
                muscleGroups,
                difficulty,
                pictureUrl,
                videoUrl,
                description,
                equipment,
                sets,
                createdOn
            )
        );
    }
#pragma warning restore S107


    public Result Update(
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        string? description,
        string? videoUrl,
        string? pictureUrl,
        string? equipment,
        List<ExerciseSet> sets
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(name))),
            Ensure.NotEmptyList(
                sets,
                DomainErrors.ArgumentError.Empty(nameof(Exercise), nameof(sets))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Name = name;
        MuscleGroups = muscleGroups;
        Difficulty = difficulty;
        PictureUrl = pictureUrl;
        VideoUrl = videoUrl;
        Description = description;
        Equipment = equipment;
        ExerciseSets = sets;

        return Result.Success();
    }
}
