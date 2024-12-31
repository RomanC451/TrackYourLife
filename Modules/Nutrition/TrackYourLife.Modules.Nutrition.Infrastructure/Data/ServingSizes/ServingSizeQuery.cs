using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;

internal class ServingSizeQuery(NutritionReadDbContext dbContext)
    : GenericQuery<ServingSizeReadModel, ServingSizeId>(dbContext.ServingSizes),
        IServingSizeQuery { }
