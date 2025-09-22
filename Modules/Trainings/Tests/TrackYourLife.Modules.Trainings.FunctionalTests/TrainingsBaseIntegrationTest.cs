using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Trainings.FunctionalTests;

[Collection("Trainings Integration Tests")]
public class TrainingsBaseIntegrationTest(TrainingsFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected HttpClient HttpClient => _client;

    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(_trainingsWriteDbContext.Trainings);
        await CleanupDbSet(_trainingsWriteDbContext.TrainingExercises);
        await CleanupDbSet(_trainingsWriteDbContext.Exercises);
        await CleanupDbSet(_trainingsWriteDbContext.OngoingTrainings);
        await CleanupDbSet(_trainingsWriteDbContext.ExerciseHistories);
        await CleanupDbSet(_trainingsWriteDbContext.OutboxMessages);
    }

    protected async Task WaitForOutboxEventsToBeHandledAsync(
        CancellationToken cancellationToken = default
    )
    {
        await WaitForOutboxEventsToBeHandledAsync(
            _trainingsWriteDbContext.OutboxMessages,
            cancellationToken
        );
    }
}
