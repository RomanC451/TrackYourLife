﻿// TODO: fix it

// using Bogus;
// using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
// using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
// using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
// using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
// using TrackYourLife.SharedLib.Domain.Ids;

// namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

// public static class FoodDiaryFaker
// {
//     private static readonly Faker f = new();

//     public static FoodDiary Generate(
//         NutritionDiaryId? id = null,
//         UserId? userId = null,
//         Food? food = null,
//         float? quantity = null,
//         DateOnly? date = null,
//         MealTypes? mealType = null,
//         ServingSize? servingSize = null
//     )
//     {
//         return FoodDiary
//             .Create(
//                 id ?? NutritionDiaryId.NewId(),
//                 userId ?? UserId.NewId(),
//                 food ?? FoodFaker.Generate(),
//                 quantity ?? f.Random.Int(1, 100),
//                 date ?? DateOnly.FromDateTime(DateTime.UtcNow),
//                 mealType ?? MealTypes.Breakfast,
//                 servingSize ?? ServingSizeFaker.Generate()
//             )
//             .Value;
//     }

//     public static FoodDiaryReadModel GenerateReadModel(
//         NutritionDiaryId? id = null,
//         UserId? userId = null,
//         FoodReadModel? food = null,
//         float? quantity = null,
//         DateOnly? date = null,
//         MealTypes? mealType = null,
//         ServingSizeReadModel? servingSize = null
//     )
//     {
//         return new FoodDiaryReadModel(
//             id ?? NutritionDiaryId.NewId(),
//             userId ?? UserId.NewId(),
//             quantity ?? f.Random.Int(1, 100),
//             mealType ?? MealTypes.Breakfast,
//             date ?? DateOnly.FromDateTime(DateTime.UtcNow),
//             DateTime.UtcNow
//         )
//         {
//             ServingSize = servingSize ?? ServingSizeFaker.GenerateReadModel(),
//             Food = food ?? FoodFaker.GenerateReadModel()
//         };
//     }
// }