using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data;

internal sealed class TrainingsUnitOfWork(TrainingsWriteDbContext dbContext)
    : UnitOfWork<TrainingsWriteDbContext>(dbContext),
        ITrainingsUnitOfWork;
