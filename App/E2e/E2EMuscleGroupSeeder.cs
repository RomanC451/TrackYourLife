using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;

namespace TrackYourLife.App.E2e;

internal static class E2EMuscleGroupSeeder
{
    private static readonly string[] DefaultGroups =
    [
        "Chest",
        "Back",
        "Legs",
        "Arms",
        "Core",
        "Shoulders",
        "Triceps",
        "Biceps",
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TrainingsWriteDbContext>();

        if (await dbContext.MuscleGroups.AnyAsync(cancellationToken))
        {
            return;
        }

        foreach (var name in DefaultGroups)
        {
            dbContext.MuscleGroups.Add(MuscleGroup.Create(MuscleGroupId.NewId(), name, null));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
