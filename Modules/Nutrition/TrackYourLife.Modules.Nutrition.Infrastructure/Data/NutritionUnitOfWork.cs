﻿using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data;

internal sealed class NutritionUnitOfWork(NutritionWriteDbContext dbContext)
    : UnitOfWork<NutritionWriteDbContext>(dbContext),
        INutritionUnitOfWork;
