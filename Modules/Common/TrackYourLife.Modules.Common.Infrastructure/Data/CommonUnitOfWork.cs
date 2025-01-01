﻿
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.Data;

internal class CommonUnitOfWork(CommonWriteDbContext dbContext)
    : UnitOfWork<CommonWriteDbContext>(dbContext),
        ICommonUnitOfWork;