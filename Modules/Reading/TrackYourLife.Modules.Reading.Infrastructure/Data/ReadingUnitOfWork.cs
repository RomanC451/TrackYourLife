using TrackYourLife.Modules.Reading.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data;

internal sealed class ReadingUnitOfWork(ReadingWriteDbContext dbContext)
    : UnitOfWork<ReadingWriteDbContext>(dbContext),
        IReadingUnitOfWork;
