using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;

internal sealed class ExercisesHistoriesRepository(TrainingsWriteDbContext dbContext)
    : GenericRepository<ExerciseHistory, ExerciseHistoryId>(dbContext.ExerciseHistories),
        IExercisesHistoriesRepository { }
