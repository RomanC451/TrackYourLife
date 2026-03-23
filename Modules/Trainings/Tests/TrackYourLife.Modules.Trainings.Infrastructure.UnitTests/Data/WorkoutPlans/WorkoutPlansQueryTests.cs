using FluentAssertions;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.WorkoutPlans;

public class WorkoutPlansQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private WorkoutPlansQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new WorkoutPlansQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_WhenActivePlanExists_ShouldReturnIt()
    {
        var userId = UserId.NewId();
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise }, userId: userId);
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var inactivePlan = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                userId,
                "Inactive",
                false,
                [training.Id],
                DateTime.UtcNow.AddDays(-2)
            )
            .Value;

        var activePlan = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                userId,
                "Active",
                true,
                [training.Id],
                DateTime.UtcNow.AddDays(-1)
            )
            .Value;

        await WriteDbContext.WorkoutPlans.AddRangeAsync(inactivePlan, activePlan);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            var result = await _sut.GetActiveByUserIdAsync(userId, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(activePlan.Id);
            result.IsActive.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnAllPlansForUser()
    {
        var userId = UserId.NewId();
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise }, userId: userId);
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var planA = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                userId,
                "A",
                false,
                [training.Id],
                DateTime.UtcNow
            )
            .Value;

        var planB = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                userId,
                "B",
                false,
                [training.Id],
                DateTime.UtcNow
            )
            .Value;

        await WriteDbContext.WorkoutPlans.AddRangeAsync(planA, planB);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            var result = (await _sut.GetByUserIdAsync(userId, CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.UserId == userId);
            result.Select(p => p.Id).Should().Contain(planA.Id);
            result.Select(p => p.Id).Should().Contain(planB.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
