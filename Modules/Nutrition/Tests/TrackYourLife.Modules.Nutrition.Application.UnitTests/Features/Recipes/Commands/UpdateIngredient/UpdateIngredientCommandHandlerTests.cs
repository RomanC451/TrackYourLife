using FluentAssertions;
using NSubstitute;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandlerTests
{
    private readonly UpdateIngredientCommandHandler _handler;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IServingSizeRepository _servingSizeRepository;
    private readonly IRecipeService _recipeService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UserId _userId;

    public UpdateIngredientCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _foodRepository = Substitute.For<IFoodRepository>();
        _servingSizeRepository = Substitute.For<IServingSizeRepository>();
        _recipeService = Substitute.For<IRecipeService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new UpdateIngredientCommandHandler(
            _userIdentifierProvider,
            _recipeRepository,
            _foodRepository,
            _servingSizeRepository,
            _recipeService
        );
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldUpdateIngredient()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var ingredient = recipe.Ingredients[0];
        var oldServingSize = ServingSizeFaker.Generate();
        var newServingSize = ServingSizeFaker.Generate();
        var food = FoodFaker.Generate(
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, servingSize: oldServingSize),
                FoodServingSizeFaker.Generate(1, servingSize: newServingSize),
            ]
        );
        var command = new UpdateIngredientCommand(
            recipe.Id,
            ingredient.Id,
            newServingSize.Id,
            2.0f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _foodRepository.GetByIdAsync(ingredient.FoodId, default).Returns(food);
        _servingSizeRepository
            .GetByIdAsync(ingredient.ServingSizeId, default)
            .Returns(oldServingSize);
        _servingSizeRepository.GetByIdAsync(command.ServingSizeId, default).Returns(newServingSize);
        _recipeService.CloneIfUsed(recipe, _userId, default).Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _recipeRepository.Received(1).Update(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateIngredientCommand(
            RecipeId.NewId(),
            IngredientId.NewId(),
            ServingSizeId.NewId(),
            1.0f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotFound");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var otherUserId = UserId.NewId();
        var recipe = RecipeFaker.Generate(userId: otherUserId);
        var ingredient = recipe.Ingredients[0];
        var command = new UpdateIngredientCommand(
            recipe.Id,
            ingredient.Id,
            ServingSizeId.NewId(),
            1.0f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotOwned");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var ingredient = recipe.Ingredients[0];
        var food = FoodFaker.Generate();
        var oldServingSize = ServingSizeFaker.Generate();
        var command = new UpdateIngredientCommand(
            recipe.Id,
            ingredient.Id,
            ServingSizeId.NewId(),
            1.0f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _foodRepository.GetByIdAsync(ingredient.FoodId, default).Returns(food);
        _servingSizeRepository
            .GetByIdAsync(ingredient.ServingSizeId, default)
            .Returns(oldServingSize);
        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, default)
            .Returns((ServingSize?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServingSize.NotFound");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var ingredient = recipe.Ingredients[0];
        var food = FoodFaker.Generate();
        var oldServingSize = ServingSizeFaker.Generate();
        var newServingSize = ServingSizeFaker.Generate();
        var command = new UpdateIngredientCommand(
            recipe.Id,
            ingredient.Id,
            newServingSize.Id,
            2.0f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _foodRepository.GetByIdAsync(ingredient.FoodId, default).Returns(food);
        _servingSizeRepository
            .GetByIdAsync(ingredient.ServingSizeId, default)
            .Returns(oldServingSize);
        _servingSizeRepository.GetByIdAsync(command.ServingSizeId, default).Returns(newServingSize);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Food.ServingSizeNotFound");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
