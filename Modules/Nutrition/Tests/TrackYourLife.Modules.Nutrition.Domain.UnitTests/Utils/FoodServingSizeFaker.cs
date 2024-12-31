// TODO: fix it

// using Bogus;
// using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
// using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
// using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

// namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

// public static class FoodServingSizeFaker
// {

//     public static FoodServingSize Generate(
//         int index,
//         FoodId? foodId = null,
//         ServingSize? servingSize = null
//     )
//     {
//         servingSize ??= ServingSizeFaker.Generate();

//         return FoodServingSize
//             .Create(foodId ?? FoodId.NewId(), servingSize.Id, servingSize, index)
//             .Value;
//     }

//     public static FoodServingSizeReadModel GenerateReadModel(
//         int index,
//         FoodId? foodId = null,
//         ServingSizeReadModel? servingSize = null
//     )
//     {
//         servingSize ??= ServingSizeFaker.GenerateReadModel();

//         return new FoodServingSizeReadModel(foodId ?? FoodId.NewId(), servingSize.Id, index)
//         {
//             ServingSize = servingSize
//         };
//     }
// }
