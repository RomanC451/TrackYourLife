using System.Text.Json;
using TrackYourLife.Modules.Trainings.Domain.Features.Sets;
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
    public string? PictureUrl { get; private set; } = string.Empty;
    public string? VideoUrl { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public string? Equipment { get; private set; } = string.Empty;
    public string ExerciseSetsJson { get; private set; } = "[]";
    public List<ExerciseSet> ExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(ExerciseSetsJson) ?? new();
        private set => ExerciseSetsJson = JsonSerializer.Serialize(value);
    }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; } = null;

    private Exercise()
        : base() { }

    private Exercise(
        ExerciseId id,
        UserId userId,
        string name,
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
                pictureUrl,
                videoUrl,
                description,
                equipment,
                sets,
                createdOn
            )
        );
    }

    public Result Update(
        string name,
        string? description,
        string? videoUrl,
        string? pictureUrl,
        string? equipment,
        List<ExerciseSet> sets,
        DateTime modifiedOn
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
        PictureUrl = pictureUrl;
        VideoUrl = videoUrl;
        Description = description;
        Equipment = equipment;
        ExerciseSets = sets;

        ModifiedOnUtc = modifiedOn;

        return Result.Success();
    }
}
