﻿using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

internal sealed record AddRecipeDiaryRequest(
    RecipeId RecipeId,
    MealTypes MealType,
    float Quantity,
    DateOnly EntryDate
);

internal sealed class AddRecipeDiary(ISender sender) : Endpoint<AddRecipeDiaryRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<RecipeDiariesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        AddRecipeDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new AddRecipeDiaryCommand(
                req.RecipeId,
                req.MealType,
                req.Quantity,
                req.EntryDate
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.FoodDiaries}/{id.Value}");
    }
}
